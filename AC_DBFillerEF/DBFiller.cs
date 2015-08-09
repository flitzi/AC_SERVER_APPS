using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AC_SessionReport;

namespace AC_DBFillerEF
{
    public class DBFiller : ISessionReportHandler
    {
        public const string Version = "0.9.1";


        //private static int[] ChampionshipPoints = new int[] { 25, 18, 15, 12, 10, 8, 6, 4, 2, 1 };
        public void HandleReport(SessionReport report)
        {
            AC_DBEntities entities = new AC_DBEntities();
            using (DbContextTransaction transaction = entities.Database.BeginTransaction())
            {
                try
                {
                    Session session = new Session();
                    session.Server = report.ServerName;
                    session.Name = report.SessionName;
                    session.Type = report.Type;
                    session.Track = report.TrackName + (string.IsNullOrEmpty(report.TrackConfig) ? string.Empty : (" " + report.TrackConfig));
                    session.LapCount = report.RaceLaps;
                    session.Time = report.Time;
                    session.Ambient = report.AmbientTemp;
                    session.Road = report.RoadTemp;
                    session.Weather = report.Weather;
                    session.Timestamp = new DateTime(report.Timestamp, DateTimeKind.Utc);

                    entities.Sessions.Add(session);

                    Dictionary<int, Driver> driverDict = new Dictionary<int, Driver>();
                    Dictionary<int, DriverReport> driverReportDict = new Dictionary<int, DriverReport>();
                    foreach (DriverReport connection in report.Connections)
                    {
                        Driver driver = entities.Drivers.FirstOrDefault(d => d.SteamId == connection.SteamId);
                        if (driver == null)
                        {
                            driver = new Driver();
                            driver.SteamId = connection.SteamId;
                            entities.Drivers.Add(driver);
                        }

                        driver.Name = connection.Name;
                        driver.Team = connection.Team;
                        driver.IncidentCount += connection.Incidents;
                        driver.Distance += (int)connection.Distance;
                        driver.Points += 0; //TODO?

                        driverDict.Add(connection.ConnectionId, driver);
                        driverReportDict.Add(connection.ConnectionId, connection);

                        Result result = new Result();
                        result.Session = session;
                        result.Driver = driver;
                        result.Car = connection.CarModel;
                        result.StartPosition = connection.StartPosition;
                        result.Position = connection.Position;
                        result.IncidentCount = connection.Incidents;
                        result.Distance = (int)connection.Distance;
                        result.LapCount = connection.LapCount;
                        result.Gap = connection.Gap;
                        result.TopSpeed = (short)Math.Round(connection.TopSpeed);

                        entities.Results.Add(result);
                    }

                    foreach (LapReport lapReport in report.Laps)
                    {
                        Lap lap = new Lap();
                        lap.Session = session;
                        lap.Driver = driverDict[lapReport.ConnectionId];
                        lap.Car = driverReportDict[lapReport.ConnectionId].CarModel;
                        lap.LapNo = lapReport.LapNo;
                        lap.Time = lapReport.LapTime;
                        lap.Cuts = lapReport.Cuts;
                        lap.Position = lapReport.Position;
                        lap.Grip = lapReport.Grip;
                        lap.Timestamp = new DateTime(lapReport.Timestamp, DateTimeKind.Utc);

                        entities.Laps.Add(lap);
                    }

                    foreach (IncidentReport incidentReport in report.Events)
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