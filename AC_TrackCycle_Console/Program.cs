using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using acPlugins4net;
using acPlugins4net.helpers;
using AC_ServerStarter;
using System.Text;
using acPlugins4net.configuration;
using AC_SessionReportPlugin;

namespace AC_TrackCycle_Console
{
    /// <summary>
    /// A static helper class Mono related.
    /// </summary>
    public static class MonoHelper
    {
        /// <summary>
        /// Whether this program is running on mono.
        /// </summary>
        public static readonly bool IsRunningMono;
        /// <summary>
        /// Whether this program is running on linux.
        /// </summary>
        public static readonly bool IsLinux;

        static MonoHelper()
        {
            IsRunningMono = Type.GetType("Mono.Runtime") != null;
            int p = (int)Environment.OSVersion.Platform;
            IsLinux = (p == 4) || (p == 6) || (p == 128);
        }
    }

    internal static class Program
    {
        private static TrackCyclePlugin trackCycler;

        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        private static EventHandler _handler;

        private enum CtrlType
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
        private static void Main()
        {
            try
            {
                FileLogWriter logWriter = null;

                IConfigManager config = new AppConfigConfigurator();
                if (config.GetSettingAsInt("log_to_console", 0) == 0)
                {
                    if (config.GetSettingAsInt("overwrite_log_file", 0) == 1)
                    {
                        logWriter = new FileLogWriter(
                                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "logs"),
                                "TrackCycle.log")
                        { LogWithTimestamp = true };
                    }
                    else
                    {
                        logWriter = new FileLogWriter(
                                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "logs"),
                                DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + "_Startup.log")
                        { LogWithTimestamp = true };
                    }
                }

                try
                {
                    trackCycler = new TrackCyclePlugin();

                    AcServerPluginManager pluginManager = new AcServerPluginManager(logWriter, config);
                    pluginManager.LoadInfoFromServerConfig();
                    pluginManager.AddPlugin(trackCycler);
                    pluginManager.LoadPluginsFromAppConfig();

                    if (!MonoHelper.IsLinux)
                    {
                        // Some boilerplate to react to close window event, CTRL-C, kill, etc
                        _handler += new EventHandler(Handler);
                        SetConsoleCtrlHandler(_handler, true);
                    }

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
                            trackCycler.NextTrackAsync(true);
                        }
                        else
                        {
                            pluginManager.BroadcastChatMessage(line);
                        }
                    }

                    trackCycler.StopServer();
                }
                catch (Exception ex)
                {
                    if (logWriter != null)
                    {
                        logWriter.Log(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                if (logWriter != null)
                {
                    logWriter.StopLoggingToFile();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}