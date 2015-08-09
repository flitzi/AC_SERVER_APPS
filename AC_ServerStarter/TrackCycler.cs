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

namespace AC_ServerStarter
{
    public delegate void MessageReceived(string message);

    public delegate void TrackChanged(RaceSession session);

    public class TrackCycler
    {
        public const string Version = "2.1.0";

        private readonly string serverfolder, serverExe, server_cfg, entry_list;
        private readonly object lockObject = new object();
        private readonly string[] iniLines = new string[0];
        private readonly List<object> Sessions = new List<object>();
        private readonly AcServerPluginManager pluginManager;
        private readonly IFileLog logWriter;

        //private readonly List<string> admins = new List<string>(); //not used, you have to pass the admin password everytime for /next_track and /change_track, e.g. "/next_track <mypassword>" or /change_track <mypassword> spa
        private bool adminpwSet;
        private string next_trackCommand, change_trackCommand, send_chatCommand;

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

        public TrackCycler(AcServerPluginManager pluginManager, IFileLog logWriter)
        {
            this.serverfolder = pluginManager.Config.GetSetting("ac_server_directory");
            if (string.IsNullOrWhiteSpace(this.serverfolder))
            {
                this.serverfolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }

            this.serverExe = pluginManager.Config.GetSetting("ac_server_executable");
            if (string.IsNullOrWhiteSpace(this.serverExe))
            {
                this.serverExe = "acServer.exe";
            }

            this.server_cfg = Path.Combine(serverfolder, "cfg", "server_cfg.ini");
            this.entry_list = Path.Combine(serverfolder, "cfg", "entry_list.ini");
            this.pluginManager = pluginManager;
            this.logWriter = logWriter;
            this.AutoChangeTrack = true;

            string servername, adminpw, track, layout;
            int laps;
            ReadCfg(pluginManager.Config, true, out servername, out adminpw, out track, out layout, out laps, ref this.Sessions, out this.iniLines);
            this.setAdminCommands(adminpw);

            if (this.Sessions.Count == 0)
            {
                string templatecycle = pluginManager.Config.GetSetting("template_cycle");
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

        private static void ReadCfg(
            IConfigManager config,
            bool readTrackCycle,
            out string servername,
            out string adminpw,
            out string track,
            out string layout,
            out int laps,
            ref List<object> sessions,
            out string[] iniLines)
        {
            WorkaroundHelper helper = new WorkaroundHelper(config);

            iniLines = helper.ConfigIni;

            if (!helper.TryFindServerConfigEntry("ADMIN_PASSWORD=", out adminpw))
            {
                adminpw = "pippo";
            }

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

        private void setAdminCommands(string adminpw)
        {
            this.adminpwSet = !string.IsNullOrWhiteSpace(adminpw);
            this.next_trackCommand = "ADMIN COMMAND: /next_track " + adminpw;
            this.change_trackCommand = "ADMIN COMMAND: /change_track " + adminpw;
            this.send_chatCommand = "ADMIN COMMAND: /send_chat " + adminpw;
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

            string servername, adminpw, track, layout;
            int laps;
            List<object> tmpS = new List<object>();
            string[] tmpL;

            ReadCfg(this.pluginManager.Config, false, out servername, out adminpw, out track, out layout, out laps, ref tmpS, out tmpL);
            this.setAdminCommands(adminpw);

            if (this.pluginManager != null)
            {
                this.pluginManager.ServerName = servername;
                this.pluginManager.Track = track;
                this.pluginManager.TrackLayout = layout;
                //this.pluginManager.MaxClients = maxClients;

                this.pluginManager.Connect();
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
                        if (this.logWriter != null)
                        {
                            this.logWriter.Log(message);
                        }

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

                        if (adminpwSet)
                        {
                            if (message.StartsWith(this.next_trackCommand))
                            {
                                this.NextTrack();
                            }

                            if (message.StartsWith(this.change_trackCommand))
                            {
                                string track = message.Substring(this.change_trackCommand.Length).Trim();
                                string layout = string.Empty;
                                track = track.Substring(0, track.IndexOf(" "));
                                string[] parts = track.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length == 2)
                                {
                                    track = parts[0];
                                    layout = parts[1];
                                }
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
                                if (index > -1)
                                {
                                    this.ChangeTrack(index);
                                }
                            }

                            if (message.StartsWith(this.send_chatCommand))
                            {
                                string msg = message.Substring(this.send_chatCommand.Length).Trim();
                                int endix = msg.IndexOf(" received from ");
                                if (endix > 0)
                                {
                                    msg = msg.Substring(0, endix);
                                }
                                if (this.pluginManager != null && this.pluginManager.IsConnected)
                                {
                                    this.pluginManager.BroadcastChatMessage(msg);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (this.logWriter != null)
                    {
                        this.logWriter.Log(ex);
                    }
                }
            }
        }

        public void ChangeTrack(int index)
        {
            if (this.HasCycle)
            {
                if (this.pluginManager != null && this.pluginManager.IsConnected)
                {
                    Thread.Sleep(1000);
                    this.pluginManager.BroadcastChatMessage("TRACK CHANGE INCOMING, PLEASE EXIT and RECONNECT");
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

            if (this.pluginManager != null && this.pluginManager.IsConnected)
            {
                this.pluginManager.Disconnect();
            }
        }
    }
}