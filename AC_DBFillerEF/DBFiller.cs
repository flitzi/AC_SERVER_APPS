using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using acPlugins4net.info;
using acPlugins4net.helpers;

namespace AC_DBFillerEF
{
    public class DBFiller : ISessionReportHandler
    {
        public const string Version = "0.9.1";


        //private static int[] ChampionshipPoints = new int[] { 25, 18, 15, 12, 10, 8, 6, 4, 2, 1 };
        public void HandleReport(SessionInfo report)
        {
            AC_DBEntities entities = new AC_DBEntities();
            using (DbContextTransaction transaction = entities.Database.BeginTransaction())
            {
                try
                {
                    Session session = new Session();
                    session.Server = report.ServerName;
                    session.Name = report.SessionName;
                    session.Type = report.SessionType;
                    session.Track = report.TrackName + (string.IsNullOrEmpty(report.TrackConfig) ? string.Empty : (" " + report.TrackConfig));
                    session.LapCount = (short)report.LapCount;
                    session.Time = report.SessionDuration;
                    session.Ambient = report.AmbientTemp;
                    session.Road = report.RoadTemp;
                    session.Weather = report.Weather;
                    session.Timestamp = new DateTime(report.Timestamp, DateTimeKind.Utc);

                    entities.Sessions.Add(session);

                    Dictionary<int, Driver> driverDict = new Dictionary<int, Driver>();
                    Dictionary<int, DriverInfo> driverReportDict = new Dictionary<int, DriverInfo>();
                    foreach (DriverInfo connection in report.Drivers)
                    {
                        Driver driver = entities.Drivers.FirstOrDefault(d => d.SteamId == connection.DriverGuid);
                        if (driver == null)
                        {
                            driver = new Driver();
                            driver.SteamId = connection.DriverGuid;
                            entities.Drivers.Add(driver);
                        }

                        driver.Name = connection.DriverName;
                        driver.Team = connection.DriverTeam;
                        driver.IncidentCount += connection.Incidents;
                        driver.Distance += (int)connection.Distance;
                        driver.Points += 0; //TODO?

                        driverDict.Add(connection.ConnectionId, driver);
                        driverReportDict.Add(connection.ConnectionId, connection);

                        Result result = new Result();
                        result.Session = session;
                        result.Driver = driver;
                        result.Car = connection.CarModel;
                        result.StartPosition = (short)connection.StartPosition;
                        result.Position = (short)connection.Position;
                        result.IncidentCount = connection.Incidents;
                        result.Distance = (int)connection.Distance;
                        result.LapCount = (short)connection.LapCount;
                        result.Gap = connection.Gap;
                        result.TopSpeed = (short)Math.Round(connection.TopSpeed);

                        entities.Results.Add(result);
                    }

                    foreach (LapInfo lapReport in report.Laps)
                    {
                        Lap lap = new Lap();
                        lap.Session = session;
                        lap.Driver = driverDict[lapReport.ConnectionId];
                        lap.Car = driverReportDict[lapReport.ConnectionId].CarModel;
                        lap.LapNo = (short)lapReport.LapNo;
                        lap.Time = (int)lapReport.Laptime;
                        lap.Cuts = lapReport.Cuts;
                        lap.Position = (short)lapReport.Position;
                        lap.Grip = lapReport.GripLevel;
                        lap.Timestamp = new DateTime(lapReport.Timestamp, DateTimeKind.Utc);

                        entities.Laps.Add(lap);
                    }

                    foreach (IncidentInfo incidentReport in report.Incidents)
                    {
                        Incident incident = new Incident();
                        incident.Session = session;
                        incident.Type = incidentReport.Type;
                        incident.RelativeSpeed = incidentReport.ImpactSpeed;
                        incident.Timestamp = new DateTime(incidentReport.Timestamp, DateTimeKind.Utc);
                        incident.Driver1 = driverDict[incidentReport.ConnectionId1];
                        incident.Driver2 = incidentReport.ConnectionId2 >= 0 ? driverDict[incidentReport.ConnectionId2] : null;
                        incident.WorldPosX = incidentReport.WorldPosition.X;
                        incident.WorldPosY = incidentReport.WorldPosition.Y;
                        incident.WorldPosZ = incidentReport.WorldPosition.Z;

                        entities.Incidents.Add(incident);
                    }

                    entities.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}