using AC_SessionReport;
using acPlugins4net;
using acPlugins4net.kunos;
using acPlugins4net.messages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AC_SessionReportPlugin
{
    public class ReportPlugin : AcServerPlugin
    {
        public readonly LogWriter LogWriter;

        private int nextConnectionId = 1;
        private SessionReport currentSession = new SessionReport();

        private Dictionary<byte, DriverReport> carUsedByDictionary = new Dictionary<byte, DriverReport>();

        public readonly List<ISessionReportHandler> SessionReportHandlers = new List<ISessionReportHandler>();

        public string ServerName
        {
            get
            {
                return this.currentSession.ServerName;
            }
            set
            {
                this.currentSession.ServerName = value;
            }
        }

        public string CurrentTrack
        {
            get
            {
                return this.currentSession.TrackName;
            }
            set
            {
                this.currentSession.TrackName = value;
            }
        }

        public string CurrentTrackLayout
        {
            get
            {
                return this.currentSession.TrackConfig;
            }
            set
            {
                this.currentSession.TrackConfig = value;
            }
        }

        public int BroadcastIncidents { get; set; }

        public int BroadcastResults { get; set; }

        public ReportPlugin(LogWriter logWriter)
        {
            this.LogWriter = logWriter;
            this.BroadcastIncidents = this.Config.GetSettingAsInt("BroadcastIncidents", 0);
            this.BroadcastResults = this.Config.GetSettingAsInt("BroadcastResults", 0);
        }

        public bool SessionHasInfo()
        {
            return this.currentSession.Laps.Count > 0;
        }

        public override void Disconnect()
        {
            base.Disconnect();
            this.OnNewSession(null);
            this.carUsedByDictionary.Clear();
            this.currentSession = new SessionReport();
        }

        public override void OnNewSession(MsgNewSession msg)
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
                        else if (currentSession.Type != 3)
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

                    if (currentSession.Type == 3) //if race
                    {
                        short position = 1;
                        int winnerlapcount = 0;
                        int winnertime = 0;
                        // might be incorrect for players connected after race started
                        foreach (DriverReport connection in this.currentSession.Connections.OrderByDescending(d => d.LapCount).ThenBy(d => this.currentSession.Laps.Where(l => l.ConnectionId == d.ConnectionId && l.LapNo == d.LapCount).First().TimeStamp))
                        {
                            if (position == 1)
                            {
                                winnerlapcount = connection.LapCount;
                                winnertime = connection.TotalTime;
                            }
                            connection.Position = position++;

                            if (connection.LapCount == winnerlapcount)
                            {
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
                        this.BroadcastChatMessage("Pos  Name\tCar\tGap\tBestLap\tIncidents");
                        foreach (DriverReport d in this.currentSession.Connections.OrderBy(d => d.Position).Take(this.BroadcastResults))
                        {
                            this.BroadcastChatMessage(string.Format("{0}   {1}\t{2}\t{3}\t{4}\t{5}", d.Position.ToString("00"), d.Name, d.CarModel, d.Gap, FormatTimespan(d.BestLap), d.Incidents));
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
                            this.OnError(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
            finally
            {
                SessionReport oldSession = this.currentSession;

                if (msg != null)
                {
                    this.currentSession = new SessionReport()
                    {
                        ServerName = oldSession.ServerName,
                        TrackName = oldSession.TrackName,
                        TrackConfig = oldSession.TrackConfig,
                        SessionName = msg.Name,
                        Type = msg.SessionType,
                        DurationSecs = msg.TimeOfDay,
                        RaceLaps = (short)msg.Laps,
                        TimeStamp = DateTime.UtcNow.Ticks,
                        AmbientTemp = msg.AmbientTemp,
                        RoadTemp = msg.RoadTemp,
                        Weather = msg.Weather
                    };
                }
                else
                {
                    this.currentSession = new SessionReport();
                }

                this.nextConnectionId = 1;

                foreach (DriverReport connection in oldSession.Connections)
                {
                    DriverReport found;
                    if (carUsedByDictionary.TryGetValue(connection.CarId, out found) && found == connection)
                    {
                        DriverReport recreatedConnection = new DriverReport()
                        {
                            ConnectionId = nextConnectionId++,
                            ConnectedTimeStamp = found.ConnectedTimeStamp,
                            DisconnectedTimeStamp = found.DisconnectedTimeStamp, // should be not set yet
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

            if (msg != null)
            {
                EnableRealtimeReport(1000);

                if (LogWriter != null)
                {
                    LogWriter.StartLoggingToFile(this.currentSession);
                }
            }
        }

        public override void OnNewConnection(MsgNewConnection msg)
        {
            DriverReport newConnection = new DriverReport()
            {
                ConnectionId = nextConnectionId++,
                ConnectedTimeStamp = DateTime.UtcNow.Ticks,
                DisconnectedTimeStamp = 0,
                SteamId = msg.DriverGuid,
                Name = msg.DriverName,
                Team = "NA", // missing in msg
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

            currentSession.Connections.Add(newConnection);

            if (!carUsedByDictionary.ContainsKey(newConnection.CarId))
            {
                carUsedByDictionary.Add(newConnection.CarId, newConnection);
            }
            else
            {
                carUsedByDictionary[msg.CarId] = newConnection;
                this.OnError(new Exception("Car already in used by another driver"));
            }
        }

        public override void OnConnectionClosed(MsgConnectionClosed msg)
        {
            if (!carUsedByDictionary.Remove(msg.CarId))
            {
                this.OnError(new Exception("Car was not known to be in use"));
            }
        }

        public override void OnCarUpdate(MsgCarUpdate msg)
        {
            try
            {
                DriverReport driver = carUsedByDictionary[msg.CarId];
                driver.AddDistance(msg.WorldPosition.x, msg.WorldPosition.x, msg.WorldPosition.z);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        public override void OnCollision(MsgClientEvent msg)
        {
            try
            {
                DriverReport driver = carUsedByDictionary[msg.CarId];
                bool withOtherCar = msg.Subtype == (byte)ACSProtocol.MessageType.ACSP_CE_COLLISION_WITH_CAR;

                driver.Incidents += withOtherCar ? 2 : 1; // TODO only if relVel > thresh

                DriverReport driver2 = null;
                if (withOtherCar)
                {
                    driver2 = carUsedByDictionary[msg.OtherCarId];
                    driver2.Incidents += 2; // TODO only if relVel > thresh
                }

                currentSession.Events.Add(new IncidentReport()
                {
                    Type = msg.Subtype,
                    TimeStamp = DateTime.UtcNow.Ticks,
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
                        this.BroadcastChatMessage(string.Format("Collision between {0} and {1} with {2}km/h", driver.Name, driver2.Name, Math.Round(msg.RelativeVelocity)));
                    }
                    else if (this.BroadcastIncidents > 1)
                    {
                        this.BroadcastChatMessage(string.Format("{0} crashed into wall with {1}km/h", driver.Name, Math.Round(msg.RelativeVelocity)));
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        public override void OnLapCompleted(MsgLapCompleted msg)
        {
            try
            {
                DriverReport driver = carUsedByDictionary[msg.CarId];

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

                currentSession.Laps.Add(new LapReport()
                {
                    ConnectionId = driver.ConnectionId,
                    TimeStamp = DateTime.UtcNow.Ticks,
                    LapTime = (int)msg.Laptime,
                    LapNo = (short)lapNo,
                    Position = position,
                    Cuts = msg.Cuts,
                    Grip = msg.GripLevel
                });
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }
        protected override void OnError(Exception ex)
        {
            base.OnError(ex);
            if (this.LogWriter != null)
            {
                this.LogWriter.LogException(ex);
            }
        }

        public new void BroadcastChatMessage(string msg)
        {
            base.BroadcastChatMessage(msg);
        }

        private static Single3 ToSingle3(MsgClientEvent.Vector3f vec)
        {
            return new Single3() { X = vec.x, Y = vec.y, Z = vec.z };
        }

        private static string FormatTimespan(int timespan)
        {
            int minutes = timespan / 1000 / 60;
            double seconds = (timespan - minutes * 1000 * 60) / 1000.0;
            return string.Format("{0:00}:{1:00.000}", minutes, seconds);
        }

    }
}
