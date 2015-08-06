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

namespace AC_SessionReportPlugin
{
    public class ReportPlugin : AcServerPluginBase
    {
        protected AcServerPluginManager pluginManager { get; private set; }
        public readonly List<ISessionReportHandler> SessionReportHandlers = new List<ISessionReportHandler>();
        public int BroadcastIncidents { get; set; }
        public int BroadcastResults { get; set; }
        protected readonly Dictionary<byte, DriverReport> carUsedByDictionary = new Dictionary<byte, DriverReport>();
        protected int nextConnectionId = 1;
        protected SessionReport currentSession = new SessionReport();
        protected bool loadedHandlersFromConfig;

        public virtual bool SessionHasInfo()
        {
            return this.currentSession.Laps.Count > 0;
        }

        protected virtual void FinalizeAndStartNewReport()
        {
            try
            {
                if (this.SessionHasInfo())
                {
                    // update PlayerConnections with results
                    foreach (DriverReport connection in this.currentSession.Connections)
                    {
                        List<LapReport> laps = this.currentSession.Laps.Where(l => l.ConnectionId == connection.ConnectionId).ToList();
                        List<LapReport> validLaps = laps.Where(l => l.Cuts == 0).ToList();
                        if (validLaps.Count > 0)
                        {
                            connection.BestLap = validLaps.Min(l => l.LapTime);
                        }
                        else if (this.currentSession.Type != (byte)MsgNewSession.SessionTypeEnum.Race)
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

                    if (this.currentSession.Type == (byte)MsgNewSession.SessionTypeEnum.Race) //if race
                    {
                        short position = 1;
                        int winnerlapcount = 0;
                        int winnertime = 0;
                        foreach (DriverReport connection in
                            this.currentSession.Connections.OrderByDescending(d => d.LapCount).ThenBy(this.GetLastLapTimestamp))
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
                                    connection.Gap = winnerlapcount - connection.LapCount + " laps";
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
                        this.pluginManager.BroadcastChatMessage(this.currentSession.SessionName + " Results:");
                        this.pluginManager.BroadcastChatMessage("Pos  Name\tCar\tGap\tBestLap\tIncidents");
                        foreach (DriverReport d in this.currentSession.Connections.OrderBy(d => d.Position).Take(this.BroadcastResults))
                        {
                            this.pluginManager.BroadcastChatMessage(
                                string.Format(
                                    "{0}   {1}\t{2}\t{3}\t{4}\t{5}",
                                    d.Position.ToString("00"),
                                    d.Name,
                                    d.CarModel,
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
                            this.pluginManager.Log(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.pluginManager.Log(ex);
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
                            Gap = null,
                            Incidents = 0,
                            Distance = 0.0
                        };

                        this.currentSession.Connections.Add(recreatedConnection);
                        this.carUsedByDictionary[recreatedConnection.CarId] = recreatedConnection;
                    }
                }
            }
        }

        #region AcServerPluginBase overrides
        protected override void OnInitBase(AcServerPluginManager manager)
        {
            this.pluginManager = manager;
            this.BroadcastIncidents = manager.Config.GetSettingAsInt("broadcast_incidents", 0);
            this.BroadcastResults = manager.Config.GetSettingAsInt("broadcast_results", 0);

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
            this.currentSession.ServerName = this.pluginManager.ServerName;
            this.currentSession.TrackName = this.pluginManager.Track;
            this.currentSession.TrackConfig = this.pluginManager.TrackLayout;
        }

        protected override void OnDisconnectedBase()
        {
            this.FinalizeAndStartNewReport();
            this.carUsedByDictionary.Clear();
            this.currentSession = new SessionReport();
        }

        protected override void OnNewSessionBase(MsgNewSession msg)
        {
            this.FinalizeAndStartNewReport();

            if (msg != null)
            {
                this.currentSession.ServerName = this.pluginManager.ServerName;
                this.currentSession.TrackName = this.pluginManager.Track;
                this.currentSession.TrackConfig = this.pluginManager.TrackLayout;
                this.currentSession.SessionName = msg.Name;
                this.currentSession.Type = msg.SessionType;
                this.currentSession.Time = msg.TimeOfDay;
                this.currentSession.RaceLaps = (short)msg.Laps;
                this.currentSession.Timestamp = DateTime.UtcNow.Ticks;
                this.currentSession.AmbientTemp = msg.AmbientTemp;
                this.currentSession.RoadTemp = msg.RoadTemp;
                this.currentSession.Weather = msg.Weather;
            }
            ;

            this.pluginManager.EnableRealtimeReport(1000);

            if (this.pluginManager.Logger is IFileLog)
            {
                ((IFileLog)this.pluginManager.Logger).StartLoggingToFile(
                    new DateTime(this.currentSession.Timestamp, DateTimeKind.Utc).ToString("yyyyMMdd_HHmmss") + "_"
                    + this.currentSession.TrackName + "_" + this.currentSession.SessionName + ".log");
            }
        }

        protected override void OnNewConnectionBase(MsgNewConnection msg)
        {
            DriverReport newConnection = new DriverReport()
            {
                ConnectionId = this.nextConnectionId++,
                ConnectedTimestamp = DateTime.UtcNow.Ticks,
                DisconnectedTimestamp = 0,
                SteamId = msg.DriverGuid,
                Name = msg.DriverName,
                Team = null, // missing in msg
                CarId = msg.CarId,
                CarModel = msg.CarModel,
                CarSkin = msg.CarSkin,
                BallastKG = 0, // missing in msg
                BestLap = 0,
                TotalTime = 0,
                LapCount = 0,
                Position = -1,
                Gap = null,
                Incidents = 0,
                Distance = 0.0
            };

            this.currentSession.Connections.Add(newConnection);

            if (!this.carUsedByDictionary.ContainsKey(newConnection.CarId))
            {
                this.carUsedByDictionary.Add(newConnection.CarId, newConnection);
            }
            else
            {
                this.carUsedByDictionary[msg.CarId] = newConnection;
                this.pluginManager.Log(new Exception("Car already in used by another driver"));
            }
        }

        protected override void OnConnectionClosedBase(MsgConnectionClosed msg)
        {
            if (!this.carUsedByDictionary.Remove(msg.CarId))
            {
                this.pluginManager.Log(new Exception("Car was not known to be in use"));
            }
        }

        protected override void OnCarUpdateBase(MsgCarUpdate msg)
        {
            try
            {
                DriverReport driver = this.carUsedByDictionary[msg.CarId];
                driver.AddDistance(msg.WorldPosition.x, msg.WorldPosition.x, msg.WorldPosition.z);
            }
            catch (Exception ex)
            {
                this.pluginManager.Log(ex);
            }
        }

        protected override void OnCollisionBase(MsgClientEvent msg)
        {
            try
            {
                DriverReport driver = this.carUsedByDictionary[msg.CarId];
                bool withOtherCar = msg.Subtype == (byte)ACSProtocol.MessageType.ACSP_CE_COLLISION_WITH_CAR;

                driver.Incidents += withOtherCar ? 2 : 1; // TODO only if relVel > thresh

                DriverReport driver2 = null;
                if (withOtherCar)
                {
                    driver2 = this.carUsedByDictionary[msg.OtherCarId];
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
                        this.pluginManager.BroadcastChatMessage(
                            string.Format(
                                "Collision between {0} and {1} with {2}km/h",
                                driver.Name,
                                driver2.Name,
                                Math.Round(msg.RelativeVelocity)));
                    }
                    else if (this.BroadcastIncidents > 1)
                    {
                        this.pluginManager.BroadcastChatMessage(
                            string.Format("{0} crashed into wall with {1}km/h", driver.Name, Math.Round(msg.RelativeVelocity)));
                    }
                }
            }
            catch (Exception ex)
            {
                this.pluginManager.Log(ex);
            }
        }

        protected override void OnLapCompletedBase(MsgLapCompleted msg)
        {
            try
            {
                DriverReport driver = this.carUsedByDictionary[msg.CarId];

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

                this.currentSession.Laps.Add(
                    new LapReport()
                    {
                        ConnectionId = driver.ConnectionId,
                        Timestamp = DateTime.UtcNow.Ticks,
                        LapTime = (int)msg.Laptime,
                        LapNo = (short)lapNo,
                        Position = position,
                        Cuts = msg.Cuts,
                        Grip = msg.GripLevel
                    });
            }
            catch (Exception ex)
            {
                this.pluginManager.Log(ex);
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

        private static Single3 ToSingle3(PluginMessage.Vector3f vec)
        {
            return new Single3() { X = vec.x, Y = vec.y, Z = vec.z };
        }

        private static string FormatTimespan(int timespan)
        {
            int minutes = timespan / 1000 / 60;
            double seconds = (timespan - minutes * 1000 * 60) / 1000.0;
            return string.Format("{0:00}:{1:00.000}", minutes, seconds);
        }
        #endregion
    }
}