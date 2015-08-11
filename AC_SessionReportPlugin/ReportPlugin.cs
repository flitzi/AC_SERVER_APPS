using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using acPlugins4net;
using acPlugins4net.helpers;
using acPlugins4net.kunos;
using acPlugins4net.messages;
using AC_SessionReport;
using System.IO;
using System.Threading;

namespace AC_SessionReportPlugin
{
    public class ReportPlugin : AcServerPluginBase
    {
        public const string Version = SessionReport.Version; // for now use same version

        public AcServerPluginManager PluginManager { get; private set; }
        public readonly List<ISessionReportHandler> SessionReportHandlers = new List<ISessionReportHandler>();
        public int BroadcastIncidents { get; set; }
        public int BroadcastResults { get; set; }
        public int BroadcastFastestLap { get; set; }
        public ushort RealTimeUpdateInterval { get; set; }
        public string WelcomeMessage { get; set; }

        protected readonly Dictionary<byte, DriverReport> carUsedByDictionary = new Dictionary<byte, DriverReport>();
        protected int nextConnectionId = 1;
        protected SessionReport currentSession = new SessionReport();
        protected bool loadedHandlersFromConfig;

        protected DriverReport getDriverReportForCarId(byte carId)
        {
            DriverReport driverReport;
            if (!carUsedByDictionary.TryGetValue(carId, out driverReport))
            {
                // it seems we missed the OnNewConnection for this driver
                driverReport = new DriverReport()
                {
                    ConnectionId = this.nextConnectionId++,
                    ConnectedTimestamp = DateTime.UtcNow.Ticks, //obviously not correct but better than nothing
                    DisconnectedTimestamp = 0,
                    SteamId = string.Empty,
                    Name = string.Empty,
                    Team = string.Empty,
                    CarId = carId,
                    CarModel = string.Empty,
                    CarSkin = string.Empty,
                    BallastKG = 0,
                    BestLap = 0,
                    TotalTime = 0,
                    LapCount = 0,
                    Position = -1,
                    Gap = string.Empty,
                    Incidents = 0,
                    Distance = 0.0,
                    IsAdmin = false
                };

                this.currentSession.Connections.Add(driverReport);
                this.carUsedByDictionary.Add(driverReport.CarId, driverReport);
                this.PluginManager.RequestCarInfo(carId);
            }
            else if (string.IsNullOrEmpty(driverReport.SteamId))
            {
                // it seems we did not yet receive carInfo yet, request again
                this.PluginManager.RequestCarInfo(carId);
            }

            return driverReport;
        }

        public virtual bool SessionHasInfo()
        {
            return this.currentSession.Laps.Count > 0 || this.currentSession.Events.Count > 0;
        }

        protected virtual void FinalizeAndStartNewReport()
        {
            try
            {
                if (this.SessionHasInfo())
                {
                    // if for some reason we did not get driver info for certain drivers, remove them and any associated laps and incidents
                    List<DriverReport> invalidDrivers = this.currentSession.Connections.Where(d => string.IsNullOrEmpty(d.SteamId)).ToList();
                    if (invalidDrivers.Count > 0)
                    {
                        foreach (DriverReport d in invalidDrivers)
                        {
                            this.currentSession.Connections.Remove(d);
                            this.currentSession.Laps.RemoveAll(l => l.ConnectionId == d.ConnectionId);
                            this.currentSession.Events.RemoveAll(e => e.ConnectionId1 == d.ConnectionId || e.ConnectionId2 == d.ConnectionId);
                        }
                    }

                    // update PlayerConnections with results
                    foreach (DriverReport connection in this.currentSession.Connections)
                    {
                        List<LapReport> laps = this.currentSession.Laps.Where(l => l.ConnectionId == connection.ConnectionId).ToList();
                        List<LapReport> validLaps = laps.Where(l => l.Cuts == 0).ToList();
                        if (validLaps.Count > 0)
                        {
                            connection.BestLap = validLaps.Min(l => l.LapTime);
                        }
                        else if (this.currentSession.Type != (byte)MsgSessionInfo.SessionTypeEnum.Race)
                        {
                            // temporarily set BestLap to MaxValue for easier sorting for qualifying/practice results
                            connection.BestLap = int.MaxValue;
                        }

                        if (laps.Count > 0)
                        {
                            connection.TotalTime = laps.Sum(l => l.LapTime);
                            connection.LapCount = laps.Max(l => l.LapNo);
                            connection.Incidents += laps.Sum(l => l.Cuts);
                        }
                    }

                    if (this.currentSession.Type == (byte)MsgSessionInfo.SessionTypeEnum.Race) //if race
                    {
                        short position = 1;

                        // compute start position
                        foreach (DriverReport connection in this.currentSession.Connections.Where(d => d.ConnectedTimestamp <= this.currentSession.Timestamp).OrderByDescending(d => d.StartPosNs))
                        {
                            connection.StartPosition = position++;
                        }

                        foreach (DriverReport connection in this.currentSession.Connections.Where(d => d.ConnectedTimestamp > this.currentSession.Timestamp).OrderBy(d => d.ConnectedTimestamp))
                        {
                            connection.StartPosition = position++;
                        }

                        // compute end position
                        position = 1;
                        int winnerlapcount = 0;
                        int winnertime = 0;

                        List<DriverReport> sortedDrivers = new List<DriverReport>(this.currentSession.Connections.Count);

                        sortedDrivers.AddRange(this.currentSession.Connections.Where(d => d.LapCount == currentSession.RaceLaps).OrderBy(this.GetLastLapTimestamp));
                        sortedDrivers.AddRange(this.currentSession.Connections.Where(d => d.LapCount != currentSession.RaceLaps).OrderByDescending(d => d.LapCount).ThenByDescending(d => d.LastPosNs));

                        foreach (DriverReport connection in sortedDrivers)
                        {
                            if (position == 1)
                            {
                                winnerlapcount = connection.LapCount;
                                winnertime = connection.TotalTime;
                            }
                            connection.Position = position++;

                            if (connection.LapCount == winnerlapcount)
                            {
                                // is incorrect for players connected after race started
                                connection.Gap = FormatTimespan(connection.TotalTime - winnertime);
                            }
                            else
                            {
                                if (winnerlapcount - connection.LapCount == 1)
                                {
                                    connection.Gap = "1 lap";
                                }
                                else
                                {
                                    connection.Gap = (winnerlapcount - connection.LapCount) + " laps";
                                }
                            }
                        }
                    }
                    else
                    {
                        short position = 1;
                        int winnertime = 0;
                        foreach (DriverReport connection in this.currentSession.Connections.OrderBy(d => d.BestLap))
                        {
                            if (position == 1)
                            {
                                winnertime = connection.BestLap;
                            }

                            connection.Position = position++;

                            if (connection.BestLap == int.MaxValue)
                            {
                                connection.BestLap = 0; // reset bestlap
                            }
                            else
                            {
                                connection.Gap = FormatTimespan(connection.BestLap - winnertime);
                            }
                        }
                    }

                    if (this.BroadcastResults > 0)
                    {
                        this.PluginManager.BroadcastChatMessage(this.currentSession.SessionName + " Results:");
                        this.PluginManager.BroadcastChatMessage("Pos  Name\tCar\tGap\tBestLap\tIncidents");
                        foreach (DriverReport d in this.currentSession.Connections.OrderBy(d => d.Position).Take(this.BroadcastResults))
                        {
                            this.PluginManager.BroadcastChatMessage(
                                string.Format(
                                    "{0}   {1}\t{2}\t{3}\t{4}\t{5}",
                                    d.Position.ToString("00"),
                                    d.Name.Length <= 10 ? d.Name : d.Name.Substring(0, 10),
                                    d.CarModel.Length <= 10 ? d.CarModel : d.CarModel.Substring(0, 10),
                                    d.Gap,
                                    FormatTimespan(d.BestLap),
                                    d.Incidents));
                        }
                    }

                    foreach (ISessionReportHandler handler in this.SessionReportHandlers)
                    {
                        try
                        {
                            handler.HandleReport(this.currentSession);
                        }
                        catch (Exception ex)
                        {
                            this.PluginManager.Log(ex);
                        }
                    }
                }
            }
            finally
            {
                SessionReport oldSession = this.currentSession;
                this.currentSession = new SessionReport();

                this.nextConnectionId = 1;

                foreach (DriverReport connection in oldSession.Connections)
                {
                    DriverReport found;
                    if (this.carUsedByDictionary.TryGetValue(connection.CarId, out found) && found == connection)
                    {
                        DriverReport recreatedConnection = new DriverReport()
                        {
                            ConnectionId = this.nextConnectionId++,
                            ConnectedTimestamp = found.ConnectedTimestamp,
                            DisconnectedTimestamp = found.DisconnectedTimestamp, // should be not set yet
                            SteamId = found.SteamId,
                            Name = found.Name,
                            Team = found.Team,
                            CarId = found.CarId,
                            CarModel = found.CarModel,
                            CarSkin = found.CarSkin,
                            BallastKG = found.BallastKG,
                            BestLap = 0,
                            TotalTime = 0,
                            LapCount = 0,
                            Position = -1,
                            Gap = string.Empty,
                            Incidents = 0,
                            Distance = 0.0,
                            IsAdmin = found.IsAdmin
                        };

                        this.currentSession.Connections.Add(recreatedConnection);
                    }
                }

                // clear the dictionary of cars currently used
                this.carUsedByDictionary.Clear();
                foreach (DriverReport recreatedConnection in this.currentSession.Connections)
                {
                    this.carUsedByDictionary[recreatedConnection.CarId] = recreatedConnection;
                }
            }
        }

        protected virtual string CreateWelcomeMessage(DriverReport driverReport)
        {
            if (!string.IsNullOrWhiteSpace(this.WelcomeMessage))
            {
                return this.WelcomeMessage.Replace("$DriverName$", driverReport.Name).Replace("$ServerName$", this.currentSession.ServerName);
            }
            return null;
        }

        protected virtual void SetSessionInfo(MsgSessionInfo msg, bool startNewLog)
        {
            this.currentSession.ProtocolVersion = msg.Version;
            this.currentSession.ServerName = msg.ServerName;
            this.currentSession.TrackName = msg.Track;
            this.currentSession.TrackConfig = msg.TrackConfig;
            this.currentSession.SessionName = msg.Name;
            this.currentSession.Type = msg.SessionType;
            this.currentSession.Time = msg.TimeOfDay;
            this.currentSession.RaceLaps = (short)msg.Laps;
            this.currentSession.WaitTime = msg.WaitTime;
            this.currentSession.Timestamp = DateTime.UtcNow.Ticks;
            this.currentSession.AmbientTemp = msg.AmbientTemp;
            this.currentSession.RoadTemp = msg.RoadTemp;
            this.currentSession.Weather = msg.Weather;

            if (startNewLog && this.PluginManager.Logger is IFileLog)
            {
                ((IFileLog)this.PluginManager.Logger).StartLoggingToFile(
                    new DateTime(this.currentSession.Timestamp, DateTimeKind.Utc).ToString("yyyyMMdd_HHmmss") + "_"
                    + this.currentSession.TrackName + "_" + this.currentSession.SessionName + ".log");
            }
        }

        #region AcServerPluginBase overrides
        protected override void OnInitBase(AcServerPluginManager manager)
        {
            this.PluginManager = manager;
            this.BroadcastIncidents = manager.Config.GetSettingAsInt("broadcast_incidents", 0);
            this.BroadcastResults = manager.Config.GetSettingAsInt("broadcast_results", 10);
            this.BroadcastFastestLap = manager.Config.GetSettingAsInt("broadcast_fastest_lap", 1);
            this.RealTimeUpdateInterval = (ushort)manager.Config.GetSettingAsInt("realtime_update_interval", 1000);
            this.WelcomeMessage = manager.Config.GetSetting("welcome_message");

            if (!loadedHandlersFromConfig)
            {
                loadedHandlersFromConfig = true; // only do this the first time
                string sessionReportHandlerType = manager.Config.GetSetting("session_report_handlers");
                if (!string.IsNullOrWhiteSpace(sessionReportHandlerType))
                {
                    foreach (string handlerTypeStr in sessionReportHandlerType.Split(';'))
                    {
                        string[] typeInfo = handlerTypeStr.Split(',');
                        Assembly assembly = Assembly.Load(typeInfo[1]);
                        Type type = assembly.GetType(typeInfo[0]);
                        ISessionReportHandler reportHandler = (ISessionReportHandler)Activator.CreateInstance(type);
                        this.SessionReportHandlers.Add(reportHandler);
                    }
                }
            }
        }

        protected override void OnConnectedBase()
        {
            // if we do not receive the session Info in the next 3 seconds request info (async)
            ThreadPool.QueueUserWorkItem(o =>
            {
                Thread.Sleep(3000);
                if (this.PluginManager.ProtocolVersion == -1)
                {
                    this.PluginManager.RequestSessionInfo(-1);
                }
            });
        }

        protected override void OnDisconnectedBase()
        {
            this.FinalizeAndStartNewReport();
            this.carUsedByDictionary.Clear();
            this.currentSession = new SessionReport();
        }

        protected override void OnNewSessionBase(MsgSessionInfo msg)
        {
            this.FinalizeAndStartNewReport();

            this.SetSessionInfo(msg, true);

            this.PluginManager.EnableRealtimeReport(RealTimeUpdateInterval);
        }

        protected override void OnSessionInfoBase(MsgSessionInfo msg)
        {
            bool firstSessionInfo = this.currentSession.ProtocolVersion == -1;
            if (firstSessionInfo)
            {
                // first time we received session info, also enable real time update
                this.PluginManager.EnableRealtimeReport(RealTimeUpdateInterval);
            }
            this.SetSessionInfo(msg, firstSessionInfo);
        }

        protected override void OnNewConnectionBase(MsgNewConnection msg)
        {
            DriverReport newConnection = new DriverReport()
            {
                ConnectionId = this.nextConnectionId++,
                ConnectedTimestamp = -1,
                DisconnectedTimestamp = 0,
                SteamId = msg.DriverGuid,
                Name = msg.DriverName,
                Team = string.Empty, // missing in msg
                CarId = msg.CarId,
                CarModel = msg.CarModel,
                CarSkin = msg.CarSkin,
                BallastKG = 0, // missing in msg
                BestLap = 0,
                TotalTime = 0,
                LapCount = 0,
                Position = -1,
                Gap = string.Empty,
                Incidents = 0,
                Distance = 0.0,
                IsAdmin = false
            };

            this.currentSession.Connections.Add(newConnection);

            if (!this.carUsedByDictionary.ContainsKey(newConnection.CarId))
            {
                this.carUsedByDictionary.Add(newConnection.CarId, newConnection);
            }
            else
            {
                this.carUsedByDictionary[msg.CarId] = newConnection;
                this.PluginManager.Log(new Exception("Car already in used by another driver"));
            }

            // request car info to get additional info and check when driver really is connected
            this.PluginManager.RequestCarInfo(msg.CarId);
        }

        protected override void OnCarInfoBase(MsgCarInfo msg)
        {
            DriverReport driverReport;
            if (carUsedByDictionary.TryGetValue(msg.CarId, out driverReport))
            {
                driverReport.CarModel = msg.CarModel;
                driverReport.CarSkin = msg.CarSkin;
                driverReport.Name = msg.DriverName;
                driverReport.Team = msg.DriverTeam;
                driverReport.SteamId = msg.DriverGuid;
            }
        }

        protected override void OnClientLoadedBase(MsgClientLoaded msg)
        {
            DriverReport driverReport;
            if (carUsedByDictionary.TryGetValue(msg.CarId, out driverReport) && driverReport.ConnectedTimestamp == -1)
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

        protected override void OnServerErrorBase(MsgError msg)
        {
            this.PluginManager.Log("ServerError: " + msg.ErrorMessage);
        }

        protected override void OnConnectionClosedBase(MsgConnectionClosed msg)
        {
            DriverReport driverReport;
            if (this.carUsedByDictionary.TryGetValue(msg.CarId, out driverReport))
            {
                if (msg.DriverGuid == msg.DriverGuid)
                {
                    driverReport.DisconnectedTimestamp = DateTime.UtcNow.Ticks;
                    this.carUsedByDictionary.Remove(msg.CarId);
                }
                else
                {
                    this.PluginManager.Log(new Exception("MsgOnConnectionClosed DriverGuid does not match Guid of connected driver"));
                }
            }
            else
            {
                this.PluginManager.Log(new Exception("Car was not known to be in use"));
            }
        }

        protected override void OnCarUpdateBase(MsgCarUpdate msg)
        {
            // ignore updates in the first 10 seconds of the session
            if (DateTime.UtcNow.Ticks - currentSession.Timestamp > 10 * 10000000)
            {
                DriverReport driver = this.getDriverReportForCarId(msg.CarId);
                driver.AddDistance(ToSingle3(msg.WorldPosition), ToSingle3(msg.Velocity), msg.NormalizedSplinePosition);

                //if (sw == null)
                //{
                //    sw = new StreamWriter(@"c:\workspace\positions.csv");
                //    sw.AutoFlush = true;
                //}
                //sw.WriteLine(ToSingle3(msg.WorldPosition).ToString() + ", " + ToSingle3(msg.Velocity).Length());
            }
        }

        protected override void OnCollisionBase(MsgClientEvent msg)
        {
            // ignore collisions in the first 5 seconds of the session
            if (DateTime.UtcNow.Ticks - currentSession.Timestamp > 5 * 10000000)
            {
                DriverReport driver = this.getDriverReportForCarId(msg.CarId);
                bool withOtherCar = msg.Subtype == (byte)ACSProtocol.MessageType.ACSP_CE_COLLISION_WITH_CAR;

                driver.Incidents += withOtherCar ? 2 : 1; // TODO only if relVel > thresh

                DriverReport driver2 = null;
                if (withOtherCar && msg.OtherCarId >= 0)
                {
                    driver2 = this.getDriverReportForCarId(msg.OtherCarId);
                    driver2.Incidents += 2; // TODO only if relVel > thresh
                }

                this.currentSession.Events.Add(
                    new IncidentReport()
                    {
                        Type = msg.Subtype,
                        Timestamp = DateTime.UtcNow.Ticks,
                        ConnectionId1 = driver.ConnectionId,
                        ConnectionId2 = withOtherCar ? driver2.ConnectionId : -1,
                        ImpactSpeed = msg.RelativeVelocity,
                        WorldPosition = ToSingle3(msg.WorldPosition),
                        RelPosition = ToSingle3(msg.RelativePosition),
                    });

                if (this.BroadcastIncidents > 0)
                {
                    if (withOtherCar)
                    {
                        this.PluginManager.BroadcastChatMessage(
                            string.Format(
                                "Collision between {0} and {1} with {2}km/h",
                                driver.Name,
                                driver2.Name,
                                Math.Round(msg.RelativeVelocity)));
                    }
                    else if (this.BroadcastIncidents > 1)
                    {
                        this.PluginManager.BroadcastChatMessage(
                            string.Format("{0} crashed into wall with {1}km/h", driver.Name, Math.Round(msg.RelativeVelocity)));
                    }
                }
            }
        }

        protected override void OnLapCompletedBase(MsgLapCompleted msg)
        {
            DriverReport driver = this.getDriverReportForCarId(msg.CarId);
            driver.LastPosNs = 0.0;
            byte position = 0;
            ushort lapNo = 0;
            for (int i = 0; i < msg.LeaderboardSize; i++)
            {
                if (msg.Leaderboard[i].CarId == msg.CarId)
                {
                    position = (byte)(i + 1);
                    lapNo = msg.Leaderboard[i].Laps;
                    break;
                }
            }

            LapReport lap = new LapReport()
            {
                ConnectionId = driver.ConnectionId,
                Timestamp = DateTime.UtcNow.Ticks,
                LapTime = (int)msg.Laptime,
                LapNo = (short)lapNo,
                Position = position,
                Cuts = msg.Cuts,
                Grip = msg.GripLevel
            };

            this.currentSession.Laps.Add(lap);

            // check if this is a new fastst lap for this session
            if (this.BroadcastFastestLap > 0 && lap.Cuts == 0
                && this.currentSession.Laps.FirstOrDefault(l => l.Cuts == 0 && l.LapTime < lap.LapTime) == null)
            {
                this.PluginManager.BroadcastChatMessage(
                        string.Format("{0} has set a new fastest lap: {1}", driver.Name, FormatTimespan(lap.LapTime)));
            }
        }

        protected override void OnChatMessageBase(MsgChat msg)
        {
            DriverReport driver = this.getDriverReportForCarId(msg.CarId);
            if (!driver.IsAdmin && !string.IsNullOrWhiteSpace(PluginManager.AdminPassword)
                && msg.Message.StartsWith("/admin ", StringComparison.InvariantCultureIgnoreCase))
            {
                driver.IsAdmin = msg.Message.Substring("/admin ".Length).Equals(PluginManager.AdminPassword);
            }

            if (driver.IsAdmin)
            {
                if (msg.Message.StartsWith("/send_pm ", StringComparison.InvariantCultureIgnoreCase))
                {
                    int carIdStartIdx = "/send_pm ".Length;
                    int carIdEndIdx = msg.Message.IndexOf(' ', carIdStartIdx);
                    byte carId;
                    if (carIdEndIdx > carIdStartIdx && byte.TryParse(msg.Message.Substring(carIdStartIdx, carIdEndIdx - carIdStartIdx), out carId))
                    {
                        string chatMsg = msg.Message.Substring(carIdEndIdx);
                        PluginManager.SendChatMessage(carId, chatMsg);
                    }
                    else
                    {
                        PluginManager.SendChatMessage(msg.CarId, "Invalid car id provided");
                    }
                }
                else if (msg.Message.StartsWith("/send_chat ", StringComparison.InvariantCultureIgnoreCase))
                {
                    string broadcastMsg = msg.Message.Substring("/send_chat ".Length);
                    PluginManager.BroadcastChatMessage(broadcastMsg);
                }
            }
        }
        #endregion

        #region some helper methods
        private long GetLastLapTimestamp(DriverReport driver)
        {
            LapReport lapReport =
                this.currentSession.Laps.FirstOrDefault(l => l.ConnectionId == driver.ConnectionId && l.LapNo == driver.LapCount);
            if (lapReport != null)
            {
                return lapReport.Timestamp;
            }
            return long.MaxValue;
        }

        public static Single3 ToSingle3(PluginMessage.Vector3f vec)
        {
            return new Single3() { X = vec.x, Y = vec.y, Z = vec.z };
        }

        public static string FormatTimespan(int timespan)
        {
            int minutes = timespan / 1000 / 60;
            double seconds = (timespan - minutes * 1000 * 60) / 1000.0;
            return string.Format("{0:00}:{1:00.000}", minutes, seconds);
        }
        #endregion
    }
}