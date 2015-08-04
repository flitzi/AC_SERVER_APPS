using AC_SessionReportPlugin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AC_ServerStarter
{
    public delegate void MessageReceived(string message);
    public delegate void TrackChanged(RaceSession session);

    public class TrackCycler
    {
        private readonly string serverfolder, server_cfg, entry_list;

        private readonly List<string> iniLines = new List<string>();
        private readonly List<object> Sessions = new List<object>();

        private readonly ReportPlugin plugin;
        private readonly LogWriter logWriter;

        //private readonly List<string> admins = new List<string>(); //not used, you have to pass the admin password everytime for /next_track and /change_track, e.g. "/next_track <mypassword>" or /change_track <mypassword> spa

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

        public TrackCycler(string serverfolder, ReportPlugin plugin, LogWriter logWriter)
        {
            this.serverfolder = serverfolder;
            this.server_cfg = Path.Combine(serverfolder, @"cfg\server_cfg.ini");
            this.entry_list = Path.Combine(serverfolder, @"cfg\entry_list.ini");
            this.plugin = plugin;
            this.logWriter = logWriter;
            this.AutoChangeTrack = true;

            string servername, adminpw, track, layout;
            int laps;
            ReadCfg(this.server_cfg, true, out servername, out adminpw, out track, out layout, out laps, ref this.Sessions, ref this.iniLines);
            this.setAdminCommands(adminpw);

            if (this.Sessions.Count == 0)
            {
                string templatecycle = ConfigurationManager.AppSettings["templateCycle"];
                if (!string.IsNullOrEmpty(templatecycle))
                {
                    string[] templates = templatecycle.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string template in templates)
                    {
                        string dir = Path.Combine(this.serverfolder, "cfg", template);
                        if (Directory.Exists(dir))
                        {
                            this.Sessions.Add(dir);
                        }
                    }
                }
            }
        }

        private static void ReadCfg(string cfg, bool readTrackCycle, out string servername, out string adminpw, out string track, out string layout, out int laps, ref List<object> sessions, ref List<string> iniLines)
        {
            servername = "AC server";
            adminpw = "pippo";
            track = "nurburgring";
            layout = "";
            laps = 5;

            StreamReader sr = new StreamReader(cfg);

            string line = sr.ReadLine();
            while (line != null)
            {
                if (line.StartsWith("ADMIN_PASSWORD=", StringComparison.InvariantCultureIgnoreCase))
                {
                    adminpw = line.Substring(line.IndexOf("=") + 1);
                }

                if (line.StartsWith("TRACK=", StringComparison.InvariantCultureIgnoreCase))
                {
                    track = line.Replace("TRACK=", "").Trim();
                }

                if (line.StartsWith("CONFIG_TRACK=", StringComparison.InvariantCultureIgnoreCase))
                {
                    layout = line.Replace("CONFIG_TRACK=", "").Trim();
                }

                if (line.StartsWith("LAPS=", StringComparison.InvariantCultureIgnoreCase))
                {
                    laps = int.Parse(line.Replace("LAPS=", "").Trim());
                }

                if (line.StartsWith("NAME=", StringComparison.InvariantCultureIgnoreCase))
                {
                    servername = line.Replace("NAME=", "").Trim();
                }

                if (readTrackCycle)
                {
                    if (line.StartsWith("TRACKS=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (string parts in line.Substring(line.IndexOf("=") + 1).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
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

                    iniLines.Add(line);
                }
                line = sr.ReadLine();
            }
            sr.Dispose();
        }

        private void setAdminCommands(string adminpw)
        {
            this.next_trackCommand = "ADMIN COMMAND: /next_track " + adminpw;
            this.change_trackCommand = "ADMIN COMMAND: /change_track " + adminpw;
            this.send_chatCommand = "ADMIN COMMAND: /send_chat " + adminpw;
        }

        public void StartServer()
        {
            this.StopServer();
            if (this.plugin != null && this.plugin.IsConnected)
            {
                this.plugin.Disconnect();
            }

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
            List<string> tmpL = new List<string>();

            ReadCfg(this.server_cfg, false, out servername, out adminpw, out track, out layout, out laps, ref tmpS, ref tmpL);
            this.setAdminCommands(adminpw);

            if (this.plugin != null)
            {
                this.plugin.ServerName = servername;
                this.plugin.CurrentTrack = track;
                this.plugin.CurrentTrackLayout = layout;

                this.plugin.Connect();
            }

            this.serverInstance = new Process();
            this.serverInstance.StartInfo.FileName = Path.Combine(serverfolder, "acServer.exe");
            this.serverInstance.StartInfo.WorkingDirectory = serverfolder;
            this.serverInstance.StartInfo.RedirectStandardOutput = true;
            this.serverInstance.StartInfo.UseShellExecute = false;
            this.serverInstance.StartInfo.CreateNoWindow = true;
            this.serverInstance.OutputDataReceived += process_OutputDataReceived;
            this.serverInstance.Start();
            this.serverInstance.BeginOutputReadLine();
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                string message = e.Data;

                if (!string.IsNullOrEmpty(message) && !message.StartsWith("No car with address"))
                {
                    if (this.logWriter != null)
                    {
                        this.logWriter.LogMessage(message);
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
                        if (this.plugin != null && this.plugin.IsConnected)
                        {
                            this.plugin.BroadcastChatMessage(msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.logWriter != null)
                {
                    this.logWriter.LogException(ex);
                }
            }
        }

        public void ChangeTrack(int index)
        {
            if (this.HasCycle)
            {
                if (this.plugin != null && this.plugin.IsConnected)
                {
                    this.plugin.BroadcastChatMessage("TRACK CHANGE INCOMING, PLEASE EXIT and RECONNECT");
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
        }
    }
}
