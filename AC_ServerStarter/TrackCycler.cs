using AC_SessionReportPlugin;
using System;
using System.Collections.Generic;
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
        private readonly string serverfolder;
        private readonly string servername = "unnamed server";
        private readonly List<string> iniLines = new List<string>();
        public readonly List<RaceSession> Sessions = new List<RaceSession>();
        //private readonly List<string> admins = new List<string>(); //not used, you have to pass the admin password everytime for /next_track and /change_track, e.g. "/next_track <mypassword>" or /change_track <mypassword> spa
        private readonly string next_trackCommand, change_trackCommand, send_chatCommand;
        private readonly ReportPlugin plugin;

        public RaceSession CurrentSession
        {
            get
            {
                return this.Sessions[this.cycle];
            }
        }

        public bool CreateLogs = true;
        private StreamWriter logWriter;

        private int cycle = 0;
        private Process serverInstance;

        public bool AutoChangeTrack = true;

        public event MessageReceived MessageReceived;
        public event TrackChanged TrackChanged;

        public TrackCycler(string serverfolder, ReportPlugin myPlugin)
        {
            this.serverfolder = serverfolder;
            this.plugin = myPlugin;

            StreamReader sr = new StreamReader(Path.Combine(serverfolder, @"cfg\server_cfg.ini"));

            string ctrack = "nurburgring", clayout = "";
            int claps = 5;

            string line = sr.ReadLine();
            while (line != null)
            {
                if (line.StartsWith("TRACKS=", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (string parts in line.Substring(line.IndexOf("=") + 1).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] part = parts.Split(',');
                        string track = part[0].Trim();
                        string config = string.Empty;
                        if (part.Length == 3)
                        {
                            config = part[1].Trim();
                        }
                        int laps = int.Parse(part[part.Length - 1]);
                        this.Sessions.Add(new RaceSession(track, config, laps));
                    }
                }

                if (line.StartsWith("ADMIN_PASSWORD=", StringComparison.InvariantCultureIgnoreCase))
                {
                    string adminpw = line.Substring(line.IndexOf("=") + 1);
                    this.next_trackCommand = "ADMIN COMMAND: /next_track " + adminpw;
                    this.change_trackCommand = "ADMIN COMMAND: /change_track " + adminpw;
                    this.send_chatCommand = "ADMIN COMMAND: /send_chat " + adminpw;
                }

                if (line.StartsWith("TRACK=", StringComparison.InvariantCultureIgnoreCase))
                {
                    ctrack = line.Replace("TRACK=", "").Trim();
                }

                if (line.StartsWith("CONFIG_TRACK=", StringComparison.InvariantCultureIgnoreCase))
                {
                    clayout = line.Replace("CONFIG_TRACK=", "").Trim();
                }

                if (line.StartsWith("LAPS=", StringComparison.InvariantCultureIgnoreCase))
                {
                    claps = int.Parse(line.Replace("LAPS=", "").Trim());
                }

                if (line.StartsWith("NAME=", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.servername = line.Replace("NAME=", "").Trim();
                }

                this.iniLines.Add(line);
                line = sr.ReadLine();
            }
            sr.Dispose();

            if (this.Sessions.Count == 0)
            {
                this.Sessions.Add(new RaceSession(ctrack, clayout, claps));
            }
        }

        public void StartServer()
        {
            if (this.logWriter != null)
            {
                this.logWriter.Close();
                this.logWriter.Dispose();
            }

            this.StopServer();

            if (this.cycle >= this.Sessions.Count)
            {
                this.cycle = 0;
            }

            this.plugin.OnNewSession(null);
            this.plugin.ResetAll();

            this.plugin.ServerName = this.servername;
            this.plugin.CurrentTrack = this.Sessions[cycle].Track;
            this.plugin.CurrentTrackLayout = this.Sessions[cycle].Layout;

            StreamWriter sw = new StreamWriter(Path.Combine(this.serverfolder, @"cfg\server_cfg.ini"));
            foreach (string line in this.iniLines)
            {
                string outLine = line;

                if (line.StartsWith("TRACK=", StringComparison.InvariantCultureIgnoreCase))
                {
                    outLine = "TRACK=" + this.Sessions[cycle].Track;
                }

                if (line.StartsWith("CONFIG_TRACK=", StringComparison.InvariantCultureIgnoreCase))
                {
                    outLine = "CONFIG_TRACK=" + this.Sessions[cycle].Layout;
                }

                if (line.StartsWith("LAPS=", StringComparison.InvariantCultureIgnoreCase))
                {
                    outLine = "LAPS=" + this.Sessions[cycle].Laps;
                }

                sw.WriteLine(outLine);
            }

            sw.Dispose();

            this.serverInstance = new Process();
            this.serverInstance.StartInfo.FileName = Path.Combine(serverfolder, "acServer.exe");
            this.serverInstance.StartInfo.WorkingDirectory = serverfolder;
            this.serverInstance.StartInfo.RedirectStandardOutput = true;
            this.serverInstance.StartInfo.UseShellExecute = false;
            this.serverInstance.StartInfo.CreateNoWindow = true;
            this.serverInstance.OutputDataReceived += process_OutputDataReceived;
            this.serverInstance.Start();
            this.serverInstance.BeginOutputReadLine();

            if (this.CreateLogs)
            {
                string logFile = Path.Combine(this.serverfolder, "log_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + this.Sessions[cycle].Track + ".txt");
                this.logWriter = new StreamWriter(logFile, false);
                this.logWriter.AutoFlush = true;
            }

            if (this.TrackChanged != null)
            {
                this.TrackChanged(this.Sessions[this.cycle]);
            }
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                string message = e.Data;

                if (!string.IsNullOrEmpty(message) && !message.StartsWith("No car with address"))
                {
                    if (MessageReceived != null)
                    {
                        MessageReceived(message);
                    }

                    if (this.logWriter != null)
                    {
                        this.logWriter.WriteLine(message);
                    }

                    if (this.AutoChangeTrack && message == "HasSentRaceoverPacket, move to the next session" && this.Sessions.Count > 1)
                    {
                        if (this.plugin != null)
                        {
                            this.plugin.BroadcastChatMessage("TRACK CHANGE INCOMING, PLEASE EXIT and RECONNECT");
                            Thread.Sleep(2000);
                        }
                        this.cycle++;
                        this.StartServer();
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
                        this.cycle++;
                        this.StartServer();
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
                        for (int i = 0; i < this.Sessions.Count; i++)
                        {
                            if (this.Sessions[i].Track == track && this.Sessions[i].Layout == layout)
                            {
                                this.cycle = i;
                                this.StartServer();
                                break;
                            }
                        }
                    }

                    if (message.StartsWith(this.send_chatCommand))
                    {
                        string msg = message.Substring(this.send_chatCommand.Length).Trim();
                        if (this.plugin != null)
                        {
                            this.plugin.BroadcastChatMessage(msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (MessageReceived != null)
                {
                    MessageReceived(ex.Message + " " + ex.StackTrace + Environment.NewLine);
                }
            }
        }

        public void NextTrack()
        {
            this.cycle++;
            this.StartServer();
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
