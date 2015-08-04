using System;
using System.Collections.Generic;

namespace AC_SessionReport
{
    public struct Single3
    {
        public float X, Y, Z;
    }

    public class DriverReport
    {
        private const double MaxSpeed = 150;

        public int ConnectionId { get; set; }
        public long ConnectedTimeStamp { get; set; }
        public long DisconnectedTimeStamp { get; set; }
        public string SteamId { get; set; }
        public string Name { get; set; }
        public string Team { get; set; } // currently not set
        public byte CarId { get; set; }
        public string CarModel { get; set; }
        public string CarSkin { get; set; }
        public short BallastKG { get; set; } // currently not set
        public int BestLap { get; set; }
        public int TotalTime { get; set; }
        public short LapCount { get; set; }
        public short Position { get; set; }
        public string Gap { get; set; }
        public int Incidents { get; set; }
        public double Distance { get; set; }

        private int lastTime = -1;
        private double lastPosX, lastPosY, lastPosZ;

        public void AddDistance(double x, double y, double z)
        {
            int currTime = Environment.TickCount;
            if (lastTime > 0)
            {
                double d =
                    Math.Sqrt((x - lastPosX) * (x - lastPosX)
                              + (y - lastPosY) * (y - lastPosY)
                              + (z - lastPosZ) * (z - lastPosZ));

                double speed = d / (currTime - lastTime) / 1000;

                if (speed < MaxSpeed /*&& (msg.Velocity.x != 0 || msg.Velocity.y != 0 || msg.Velocity.z != 0)*/)
                {
                    this.Distance += d;
                }
            }
            this.lastPosX = x;
            this.lastPosY = y;
            this.lastPosZ = z;
            this.lastTime = currTime;
        }
    }

    public class LapReport
    {
        public int ConnectionId { get; set; }
        public long TimeStamp { get; set; }
        public int LapTime { get; set; }
        public short LapNo { get; set; }
        public short Position { get; set; }
        public short Cuts { get; set; }
        public float Grip { get; set; }
    }

    public class IncidentReport
    {
        public byte Type { get; set; }
        public long TimeStamp { get; set; }
        public int ConnectionId1 { get; set; }
        public int ConnectionId2 { get; set; }
        public float ImpactSpeed { get; set; }
        public Single3 WorldPosition { get; set; }
        public Single3 RelPosition { get; set; }
    }

    public class SessionReport
    {
        public SessionReport()
        {
            this.Connections = new List<DriverReport>();
            this.Laps = new List<LapReport>();
            this.Events = new List<IncidentReport>();
        }

        public string ServerName { get; set; }
        public string TrackName { get; set; }
        public string TrackConfig { get; set; }
        public string SessionName { get; set; }
        public byte Type { get; set; }
        public int Time { get; set; }
        public short RaceLaps { get; set; }
        public long TimeStamp { get; set; }
        public byte AmbientTemp { get; set; }
        public byte RoadTemp { get; set; }
        public string Weather { get; set; }

        public List<DriverReport> Connections { get; set; }
        public List<LapReport> Laps { get; set; }
        public List<IncidentReport> Events { get; set; }
    }
}
