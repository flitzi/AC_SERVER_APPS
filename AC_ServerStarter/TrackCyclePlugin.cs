using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using acPlugins4net;
using acPlugins4net.helpers;
using System.Reflection;
using acPlugins4net.configuration;
using AC_SessionReportPlugin;
using acPlugins4net.messages;
using acPlugins4net.info;
using System.Linq;

namespace AC_ServerStarter
{
    public delegate void MessageReceived(string message);

    public delegate void TrackChanged(RaceSession session);

    public class TrackCyclePlugin : ReportPlugin
    {
        private string serverfolder, serverExe, server_cfg, entry_list;
        private bool createServerWindow, kick_before_change;
        private object lockObject = new object();
        private string[] iniLines = new string[0];
        public List<object> Sessions = new List<object>();
        private byte[] votes;
        private List<string> votedIds;
        private readonly List<string> additionalExes = new List<string>();
        private readonly List<Process> additionalProcesses = new List<Process>();

        public string ServerFolder
        {
            get
            {
                return this.serverfolder;
            }
        }

        public bool CreateServerWindow
        {
            get
            {
                return createServerWindow;
            }
        }

        public bool HasCycle
        {
            get
            {
                return this.Sessions.Count > 1;
            }
        }

        public bool HasAdditionalExes
        {
            get
            {
                return this.additionalExes.Count > 0;
            }
        }

        private int cycle;
        private Process serverInstance;
        public bool AutoChangeTrack { get; set; }

        protected override void OnInit()
        {
            base.OnInit();

            this.serverfolder = PluginManager.Config.GetSetting("ac_server_directory");
            if (string.IsNullOrWhiteSpace(this.serverfolder))
            {
                this.serverfolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }

            this.serverExe = PluginManager.Config.GetSetting("ac_server_executable");
            if (string.IsNullOrWhiteSpace(this.serverExe))
            {
                this.serverExe = "acServer.exe";
            }

            this.server_cfg = Path.Combine(serverfolder, "cfg", "server_cfg.ini");
            this.entry_list = Path.Combine(serverfolder, "cfg", "entry_list.ini");

            this.createServerWindow = PluginManager.Config.GetSettingAsInt("create_server_window", 0) == 1;

            this.kick_before_change = PluginManager.Config.GetSettingAsInt("kick_before_change", 0) == 1;

            string tmpExes = PluginManager.Config.GetSetting("additional_exes");
            if (!string.IsNullOrWhiteSpace(tmpExes))
            {
                foreach (string exe in tmpExes.Split(';'))
                {
                    if (!string.IsNullOrWhiteSpace(exe) && File.Exists(exe))
                    {
                        this.additionalExes.Add(exe);
                    }
                }
            }

            this.AutoChangeTrack = true;

            string servername, track, layout;
            int laps;
            ReadCfg(PluginManager.Config, true, out servername, out track, out layout, out laps, ref this.Sessions, out this.iniLines);

            if (this.Sessions.Count == 0)
            {
                string templatecycle = PluginManager.Config.GetSetting("template_cycle");
                if (!string.IsNullOrEmpty(templatecycle))
                {
                    string[] templates = templatecycle.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string template in templates)
                    {
                        string dir = Path.Combine(this.serverfolder, "cfg", template.Trim());
                        if (Directory.Exists(dir))
                        {
                            this.Sessions.Add(dir);
                        }
                    }
                }
            }
        }

        protected override void OnChatMessage(MsgChat msg)
        {
            base.OnChatMessage(msg);

            DriverInfo driver = null;
            if (this.PluginManager.TryGetDriverInfo(msg.CarId, out driver) && driver.IsAdmin)
            {
                if (msg.Message.StartsWith("/next_track", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.NextTrackAsync(true);
                }
                else if (msg.Message.StartsWith("/change_track "))
                {
                    string track = msg.Message.Substring("/change_track ".Length).Trim();
                    int trackIndex;
                    if (!int.TryParse(track, out trackIndex) || trackIndex < 0 || trackIndex > this.Sessions.Count - 1)
                    {
                        string layout = string.Empty;
                        string[] parts = track.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            track = parts[0];
                            layout = parts[1];
                        }

                        trackIndex = GetTrackIndex(track, layout);
                    }
                    if (trackIndex > -1)
                    {
                        ThreadPool.QueueUserWorkItem(o => this.ChangeTrack(trackIndex, true));
                    }
                    else
                    {
                        PluginManager.SendChatMessage(msg.CarId, "Specified track is not in trackcycle");
                    }
                }
                else if (msg.Message.StartsWith("/queue_track "))
                {
                    string track = msg.Message.Substring("/queue_track ".Length).Trim();
                    int trackIndex;
                    if (!int.TryParse(track, out trackIndex) || trackIndex < 0 || trackIndex > this.Sessions.Count - 1)
                    {
                        string layout = string.Empty;
                        string[] parts = track.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            track = parts[0];
                            layout = parts[1];
                        }

                        trackIndex = GetTrackIndex(track, layout);
                    }
                    if (trackIndex > -1)
                    {
                        this.votes = null; //disable voting
                        PluginManager.SendChatMessage(msg.CarId, "Next track will be " + this.Sessions[trackIndex]);
                        this.cycle = trackIndex - 1;
                        if (this.cycle < 0)
                        {
                            this.cycle = this.Sessions.Count - 1;
                        }
                    }
                    else
                    {
                        PluginManager.SendChatMessage(msg.CarId, "Specified track is not in trackcycle");
                    }
                }
            }
            if (msg.Message.Equals("/list_tracks", StringComparison.InvariantCultureIgnoreCase))
            {
                for (int i = 0; i < this.Sessions.Count; i++)
                {
                    PluginManager.SendChatMessage(msg.CarId, i + ": " + this.Sessions[i]);
                }
            }
            if (msg.Message.StartsWith("/vote_track ", StringComparison.InvariantCultureIgnoreCase))
            {
                if (driver == null)
                {
                    PluginManager.SendChatMessage(msg.CarId, "Sorry, you can't vote at this time, please try again later.");
                    return;
                }

                if (this.votes == null)
                {
                    PluginManager.SendChatMessage(msg.CarId, "The admin already specified the next track, please try again later.");
                    return;
                }

                if (this.votedIds.Contains(driver.DriverGuid))
                {
                    PluginManager.SendChatMessage(msg.CarId, "You can only vote once.");
                    return;
                }

                string track = msg.Message.Substring("/vote_track ".Length).Trim();
                int trackIndex;
                if (!int.TryParse(track, out trackIndex) || trackIndex < 0 || trackIndex > this.Sessions.Count - 1)
                {
                    string layout = string.Empty;
                    string[] parts = track.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        track = parts[0];
                        layout = parts[1];
                    }

                    trackIndex = GetTrackIndex(track, layout);
                }
                if (trackIndex > -1)
                {
                    this.votedIds.Add(driver.DriverGuid);
                    this.votes[trackIndex]++;
                    PluginManager.SendChatMessage(msg.CarId, "Vote registered.");
                }
                else
                {
                    PluginManager.SendChatMessage(msg.CarId, "Specified track is not in trackcycle");
                }
            }
        }

        private static void ReadCfg(
            IConfigManager config,
            bool readTrackCycle,
            out string servername,
            out string track,
            out string layout,
            out int laps,
            ref List<object> sessions,
            out string[] iniLines)
        {
            WorkaroundHelper helper = new WorkaroundHelper(config);

            iniLines = helper.ConfigIni;

            if (!helper.TryFindServerConfigEntry("TRACK=", out track))
            {
                track = "nurburgring";
            }

            if (!helper.TryFindServerConfigEntry("CONFIG_TRACK=", out layout))
            {
                layout = "";
            }

            string lapsStr;
            if (!helper.TryFindServerConfigEntry("LAPS=", out lapsStr) || !int.TryParse(lapsStr, out laps))
            {
                laps = 5;
            }

            if (!helper.TryFindServerConfigEntry("NAME=", out servername))
            {
                servername = "AC server";
            }

            string tracksStr;
            if (readTrackCycle && helper.TryFindServerConfigEntry("TRACKS=", out tracksStr))
            {
                foreach (string parts in tracksStr.Substring(tracksStr.IndexOf("=") + 1).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] part = parts.Split(',');
                    string ctrack = part[0].Trim();
                    string clayout = string.Empty;
                    if (part.Length == 3)
                    {
                        clayout = part[1].Trim();
                    }
                    int claps = int.Parse(part[part.Length - 1]);
                    sessions.Add(new RaceSession(ctrack, clayout, claps));
                }
            }
        }

        public void StartServer()
        {
            this.StopServer();

            if (this.HasCycle)
            {
                if (this.cycle >= this.Sessions.Count)
                {
                    this.cycle = 0;
                }
                if (this.Sessions[this.cycle] is RaceSession)
                {
                    RaceSession session = (RaceSession)this.Sessions[this.cycle];
                    StreamWriter sw = new StreamWriter(this.server_cfg);
                    foreach (string line in this.iniLines)
                    {
                        string outLine = line;

                        if (line.StartsWith("TRACK=", StringComparison.InvariantCultureIgnoreCase))
                        {
                            outLine = "TRACK=" + session.Track;
                        }

                        if (line.StartsWith("CONFIG_TRACK=", StringComparison.InvariantCultureIgnoreCase))
                        {
                            outLine = "CONFIG_TRACK=" + session.Layout;
                        }

                        if (line.StartsWith("LAPS=", StringComparison.InvariantCultureIgnoreCase))
                        {
                            outLine = "LAPS=" + session.Laps;
                        }

                        sw.WriteLine(outLine);
                    }
                    sw.Dispose();
                }
                else if (this.Sessions[this.cycle] is string)
                {
                    string cfgDir = (string)this.Sessions[this.cycle];

                    if (Directory.Exists(cfgDir))
                    {
                        string newcfg = Path.Combine(cfgDir, "server_cfg.ini");
                        if (File.Exists(newcfg))
                        {
                            File.Copy(newcfg, this.server_cfg, true);
                        }

                        string newentrylist = Path.Combine(cfgDir, "entry_list.ini");
                        if (File.Exists(newcfg))
                        {
                            File.Copy(newentrylist, this.entry_list, true);
                        }
                    }
                }

                this.votes = new byte[this.Sessions.Count];
                this.votedIds = new List<string>();
            }

            string servername, track, layout;
            int laps;
            List<object> tmpS = new List<object>();
            string[] tmpL;

            ReadCfg(this.PluginManager.Config, false, out servername, out track, out layout, out laps, ref tmpS, out tmpL);

            if (this.PluginManager != null)
            {
                //this.pluginManager.MaxClients = maxClients;

                this.PluginManager.Connect();
            }

            this.serverInstance = new Process();
            this.serverInstance.StartInfo.FileName = Path.Combine(this.serverfolder, this.serverExe);
            this.serverInstance.StartInfo.WorkingDirectory = this.serverfolder;
            this.serverInstance.StartInfo.RedirectStandardOutput = true;
            this.serverInstance.StartInfo.UseShellExecute = false;
            this.serverInstance.StartInfo.CreateNoWindow = !this.createServerWindow;
            this.serverInstance.OutputDataReceived += this.process_OutputDataReceived;
            this.serverInstance.Start();
            this.serverInstance.BeginOutputReadLine();

            foreach (string additionalExe in this.additionalExes)
            {
                this.additionalProcesses.Add(Process.Start(additionalExe));
            }
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            lock (this.lockObject)
            {
                try
                {
                    string message = e.Data;

                    if (!string.IsNullOrEmpty(message) && !message.StartsWith("No car with address") && !message.StartsWith("Smoothing"))
                    {
                        this.PluginManager.Log(message);

                        if (this.AutoChangeTrack && message == "Server looping")
                        {
                            int bestVote = -1;
                            int bestVoteCount = 0;

                            if (this.votes != null)
                            {
                                for (int i = 0; i < this.votes.Length; i++)
                                {
                                    if (this.votes[i] > bestVoteCount)
                                    {
                                        bestVote = i;
                                        bestVoteCount = this.votes[i];
                                    }
                                    else if (this.votes[i] == bestVoteCount)
                                    {
                                        bestVote = -1;
                                    }
                                }
                            }

                            if (bestVote != -1)
                            {
                                ThreadPool.QueueUserWorkItem(o => this.ChangeTrack(bestVote, false));
                            }
                            else
                            {
                                this.NextTrackAsync(false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.PluginManager.Log(ex);
                }
            }
        }

        public int GetTrackIndex(string track, string layout)
        {
            int index = -1;
            for (int i = 0; i < this.Sessions.Count; i++)
            {
                if (this.Sessions[i] is RaceSession)
                {
                    RaceSession session = (RaceSession)this.Sessions[i];
                    if (session.Track == track && session.Layout == layout)
                    {
                        index = i;
                        break;
                    }
                }
                else if (this.Sessions[i] is string)
                {
                    if ((string)this.Sessions[i] == track)
                    {
                        index = i;
                        break;
                    }
                }
            }

            return index;
        }

        public virtual void ChangeTrack(int index, bool broadcastResults)
        {
            if (this.HasCycle)
            {
                bool restartNeeded = true;
                if (index >= this.Sessions.Count)
                {
                    index = 0;
                }
                if (this.Sessions[this.cycle] is RaceSession && this.Sessions[index] is RaceSession
                    && ((RaceSession)this.Sessions[this.cycle]).Track == ((RaceSession)this.Sessions[index]).Track
                    && ((RaceSession)this.Sessions[this.cycle]).Layout == ((RaceSession)this.Sessions[index]).Layout
                    && ((RaceSession)this.Sessions[this.cycle]).Laps == ((RaceSession)this.Sessions[index]).Laps)
                {
                    restartNeeded = false;
                }

                this.cycle = index;

                if (restartNeeded)
                {
                    if (this.PluginManager.IsConnected)
                    {
                        if (broadcastResults)
                        {
                            this.PluginManager.RestartSession();
                        }
                        Thread.Sleep(this.PluginManager.NewSessionStartDelay + 1000);
                        this.PluginManager.BroadcastChatMessage("TRACK CHANGE INCOMING, PLEASE EXIT and RECONNECT");
                        Thread.Sleep(2000);
                        if (this.kick_before_change)
                        {
                            foreach (DriverInfo driver in this.PluginManager.GetDriverInfos().Where(d => d.IsConnected))
                            {
                                this.PluginManager.RequestKickDriverById(driver.CarId);
                            }
                            Thread.Sleep(1000);
                        }
                    }
                    this.StartServer();
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public void NextTrackAsync(bool broadcastResults)
        {
            int trackIndex = this.cycle + 1;
            ThreadPool.QueueUserWorkItem(o => this.ChangeTrack(trackIndex, broadcastResults));
        }

        public void StopServer()
        {
            foreach (Process additionalProcess in this.additionalProcesses)
            {
                if (additionalProcess != null && !additionalProcess.HasExited)
                {
                    additionalProcess.Kill();
                }
            }
            additionalProcesses.Clear();

            if (this.serverInstance != null && !this.serverInstance.HasExited)
            {
                this.PluginManager.Log("Trying to kill the acServer.exe process");
                if (this.createServerWindow)
                {
                    this.serverInstance.CloseMainWindow();
                }
                else
                {
                    this.serverInstance.Kill();
                }
                Thread.Sleep(100);
                if (this.serverInstance.HasExited)
                {
                    this.PluginManager.Log("acServer.exe process killed");
                }
                else
                {
                    this.PluginManager.Log("acServer.exe process could not be killed");
                }
            }

            if (this.PluginManager.IsConnected)
            {
                this.PluginManager.Disconnect();
                Thread.Sleep(100);
            }
        }
    }
}