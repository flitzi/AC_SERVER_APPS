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

namespace AC_ServerStarter
{
    public delegate void MessageReceived(string message);

    public delegate void TrackChanged(RaceSession session);

    public class TrackCyclePlugin : ReportPlugin
    {
        private string serverfolder, serverExe, server_cfg, entry_list;
        private object lockObject = new object();
        private string[] iniLines = new string[0];
        private List<object> Sessions = new List<object>();

        //private readonly List<string> admins = new List<string>(); //not used, you have to pass the admin password everytime for /next_track and /change_track, e.g. "/next_track <mypassword>" or /change_track <mypassword> spa

        public bool HasCycle
        {
            get
            {
                return this.Sessions.Count > 1;
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

            DriverInfo driver;
            if (this.PluginManager.TryGetDriverInfo(msg.CarId, out driver) && driver.IsAdmin)
            {
                if (msg.Message.StartsWith("/next_track", StringComparison.InvariantCultureIgnoreCase))
                {
                    ThreadPool.QueueUserWorkItem(o => this.NextTrack());
                }
                else if (msg.Message.StartsWith("/change_track "))
                {
                    string track = msg.Message.Substring("/change_track ".Length).Trim();
                    string layout = string.Empty;
                    string[] parts = track.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        track = parts[0];
                        layout = parts[1];
                    }

                    int trackIndex = GetTrackIndex(track, layout);
                    if (trackIndex > -1)
                    {
                        ThreadPool.QueueUserWorkItem(o => this.ChangeTrack(trackIndex));
                    }
                    else
                    {
                        PluginManager.SendChatMessage(msg.CarId, "Specified track is not in trackcycle");
                    }
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
            this.serverInstance.StartInfo.CreateNoWindow = true;
            this.serverInstance.OutputDataReceived += this.process_OutputDataReceived;
            this.serverInstance.Start();
            this.serverInstance.BeginOutputReadLine();
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string message = e.Data;
            ThreadPool.QueueUserWorkItem(o => this.processAsync(message));
        }

        private void processAsync(string message)
        {
            lock (this.lockObject)
            {
                try
                {
                    if (!string.IsNullOrEmpty(message) && !message.StartsWith("No car with address"))
                    {
                        this.PluginManager.Log(message);

                        if (this.AutoChangeTrack && message == "HasSentRaceoverPacket, move to the next session")
                        {
                            this.NextTrack();
                        }

                        //this is not secure, someone with the same name can exploit admin rights
                        //if (e.Data.StartsWith("Making") && e.Data.EndsWith("admin"))
                        //{
                        //    admins.Add(e.Data.Replace("Making", "").Replace("admin", "").Trim());
                        //}

                        //if (e.Data.StartsWith("ADMIN COMMAND: /next_track received from") && admins.Contains(e.Data.Replace("ADMIN COMMAND: /next_track received from", "").Trim()))
                        //{
                        //    cycle++;
                        //    StartServer();
                        //}
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

        public void ChangeTrack(int index)
        {
            if (this.HasCycle)
            {
                if (this.PluginManager.IsConnected)
                {
                    Thread.Sleep(1000);
                    this.PluginManager.BroadcastChatMessage("TRACK CHANGE INCOMING, PLEASE EXIT and RECONNECT");
                    Thread.Sleep(2000);
                }
                this.cycle = index;
                this.StartServer();
            }
        }

        public void NextTrack()
        {
            this.ChangeTrack(this.cycle + 1);
        }

        public void StopServer()
        {
            if (this.serverInstance != null && !this.serverInstance.HasExited)
            {
                this.serverInstance.Kill();
            }

            if (this.PluginManager.IsConnected)
            {
                this.PluginManager.Disconnect();
            }
        }
    }
}