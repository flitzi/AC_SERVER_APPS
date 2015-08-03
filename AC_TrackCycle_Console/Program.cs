using AC_SessionReportPlugin;
using AC_ServerStarter;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Configuration;

namespace AC_TrackCycle_Console
{
    static class Program
    {
        static TrackCycler trackCycler;

        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            if (trackCycler != null)
            {
                trackCycler.StopServer();
            }

            return true;
        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string serverfolder;
            if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "acServer.exe")))
            {
                serverfolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            else
            {
                //not used if acServer.exe is found next to this app
                serverfolder = ConfigurationManager.AppSettings["acServerDirectory"];
            }

            ReportPlugin plugin = new ReportPlugin();

            try
            {
                ReportHandlerLoader.LoadHandler(plugin);
            }
            catch
            {
                Console.WriteLine("Could not load SessionReportHandler");
            }

            plugin.Connect();

            trackCycler = new TrackCycler(serverfolder, plugin);

            if (trackCycler.Sessions.Count == 0)
            {
                Console.Out.WriteLine("No TRACKS in TRACK_CYCLE section found.");
                return;
            }

            // Some biolerplate to react to close window event, CTRL-C, kill, etc
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            //trackCycler.MessageReceived += trackCycler_MessageReceived;
            //trackCycler.TrackChanged += trackCycler_TrackChanged;          

            trackCycler.StartServer();
            Console.Out.WriteLine("Server running...");

            Console.Out.WriteLine("Write 'next_track' to cycle to the next track.");
            Console.Out.WriteLine("Write 'exit' to shut the server down.");

            while (true)
            {
                string line = Console.ReadLine();
                if (line.ToLower() == "exit")
                {
                    break;
                }
                else if (line.ToLower() == "next_track")
                {
                    trackCycler.NextTrack();
                }
                else
                {
                    plugin.BroadcastChatMessage(line);
                }
            }

            trackCycler.StopServer();

            plugin.Disconnect();
        }
    }
}
