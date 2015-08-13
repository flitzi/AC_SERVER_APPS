using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using acPlugins4net;
using acPlugins4net.helpers;
using acPlugins4net.kunos;
using acPlugins4net.messages;
using System.IO;
using System.Threading;
using acPlugins4net.info;

namespace AC_SessionReportPlugin
{
    public class ReportPlugin : AcServerPlugin
    {
        public const string Version = "2.2.0"; // for now use same version

        public int BroadcastIncidents { get; set; }
        public int BroadcastResults { get; set; }
        public int BroadcastFastestLap { get; set; }

        public string WelcomeMessage { get; set; }

        protected bool loadedHandlersFromConfig;

        protected virtual string CreateWelcomeMessage(DriverInfo driverReport)
        {
            if (!string.IsNullOrWhiteSpace(this.WelcomeMessage))
            {
                return this.WelcomeMessage.Replace("$DriverName$", driverReport.DriverName).Replace("$ServerName$", this.PluginManager.CurrentSession.ServerName);
            }
            return null;
        }

        #region AcServerPluginBase overrides
        protected override void OnInit()
        {
            this.BroadcastIncidents = this.PluginManager.Config.GetSettingAsInt("broadcast_incidents", 0);
            this.BroadcastResults = this.PluginManager.Config.GetSettingAsInt("broadcast_results", 10);
            this.BroadcastFastestLap = this.PluginManager.Config.GetSettingAsInt("broadcast_fastest_lap", 1);
            this.WelcomeMessage = this.PluginManager.Config.GetSetting("welcome_message");
        }

        protected override void OnClientLoaded(MsgClientLoaded msg)
        {
            DriverInfo driverReport;
            if (this.PluginManager.TryGetDriverInfo(msg.CarId, out driverReport))
            {
                driverReport.ConnectedTimestamp = DateTime.UtcNow.Ticks;
                string welcome = CreateWelcomeMessage(driverReport);
                if (!string.IsNullOrWhiteSpace(welcome))
                {
                    foreach (string line in welcome.Split('|'))
                    {
                        this.PluginManager.SendChatMessage(msg.CarId, line);
                    }
                }
            }
        }


        protected override void OnCollision(IncidentInfo incident)
        {
            DriverInfo driver = this.PluginManager.GetDriver(incident.ConnectionId1);

            if (incident.Type == (byte)ACSProtocol.MessageType.ACSP_CE_COLLISION_WITH_CAR)
            {
                DriverInfo driver2 = this.PluginManager.GetDriver(incident.ConnectionId2);

                this.PluginManager.BroadcastChatMessage(
                    string.Format(
                        "Collision between {0} and {1} with {2}km/h",
                        driver.DriverName,
                        driver2.DriverName,
                        Math.Round(incident.ImpactSpeed)));
            }
            else if (this.BroadcastIncidents > 1)
            {
                this.PluginManager.BroadcastChatMessage(
                    string.Format("{0} crashed into wall with {1}km/h", driver.DriverName, Math.Round(incident.ImpactSpeed)));
            }
        }

        protected override void OnNewSession(MsgSessionInfo msg)
        {
            if (this.PluginManager.PreviousSession != null
                && this.BroadcastResults > 0
                && this.PluginManager.PreviousSession.Laps.Count > 0 || this.PluginManager.PreviousSession.Incidents.Count > 0)
            {
                this.PluginManager.BroadcastChatMessage(this.PluginManager.PreviousSession.SessionName + " Results:");
                this.PluginManager.BroadcastChatMessage("Pos  Name\tCar\tGap\tBestLap\tIncidents");
                foreach (DriverInfo d in this.PluginManager.PreviousSession.Drivers.OrderBy(d => d.Position).Take(this.BroadcastResults))
                {
                    this.PluginManager.BroadcastChatMessage(
                        string.Format(
                            "{0}   {1}\t{2}\t{3}\t{4}\t{5}",
                            d.Position.ToString("00"),
                            (d.DriverName == null || d.DriverName.Length <= 10) ? d.DriverName : d.DriverName.Substring(0, 10),
                            (d.CarModel == null || d.CarModel.Length <= 10) ? d.CarModel : d.CarModel.Substring(0, 10),
                            d.Gap,
                            AcServerPluginManager.FormatTimespan((int)d.BestLap),
                            d.Incidents));
                }
            }
        }

        protected override void OnLapCompleted(LapInfo lap)
        {            
            if (this.BroadcastFastestLap > 0 && lap.Cuts == 0)
            {
                DriverInfo driver = PluginManager.GetDriver(lap.ConnectionId);
                // check if this is a new fastest lap for this session
                if (this.PluginManager.CurrentSession.Laps.FirstOrDefault(l => l.Cuts == 0 && l.Laptime < lap.Laptime) == null)
                {
                    this.PluginManager.BroadcastChatMessage(
                            string.Format("{0} has set a new fastest lap: {1}", driver.DriverName, AcServerPluginManager.FormatTimespan((int)lap.Laptime)));
                }
            }
        }
        #endregion
    }
}