using System;
using System.Collections.Generic;

namespace AC_SessionReport
{
    public struct Single3
    {
        public float X, Y, Z;

        public Single3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public static Single3 operator +(Single3 a, Single3 b)
        {
            return new Single3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Single3 operator -(Single3 a, Single3 b)
        {
            return new Single3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", X, Y, Z);
        }
    }

    public class DriverReport
    {
        public DriverReport()
        {
            this.StartPosNs = double.MinValue;
            this.LastPosNs = double.MinValue;
            this.ConnectedTimestamp = -1;
        }

        private const double MaxSpeed = 250; // m/s
        private const double MinSpeed = 1;

        public int ConnectionId { get; set; }
        public long ConnectedTimestamp { get; set; }
        public long DisconnectedTimestamp { get; set; }
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
        public short StartPosition { get; set; } // only set for race session
        public short Position { get; set; }
        public string Gap { get; set; }
        public int Incidents { get; set; }
        public double Distance { get; set; }
        public double StartPosNs { get; set; }
        public double LastPosNs { get; set; }

        private int lastTime = -1;
        private Single3 lastPos;

        public void AddDistance(Single3 pos, Single3 vel, double s)
        {
            if (StartPosNs == double.MinValue)
            {
                StartPosNs = s > 0.5 ? s - 1 : s;
            }

            int currTime = Environment.TickCount;
            if (this.lastTime > 0)
            {
                double d = (pos - lastPos).Length();

                double speed = d / (currTime - this.lastTime) / 1000;

                if (speed < MaxSpeed && vel.Length() > MinSpeed)
                {
                    this.Distance += d;
                    this.LastPosNs = s;
                }
            }
            this.lastPos = pos;

            this.lastTime = currTime;
        }
    }

    public class LapReport
    {
        public int ConnectionId { get; set; }
        public long Timestamp { get; set; }
        public int LapTime { get; set; }
        public short LapNo { get; set; }
        public short Position { get; set; }
        public short Cuts { get; set; }
        public float Grip { get; set; }
    }

    public class IncidentReport
    {
        public byte Type { get; set; }
        public long Timestamp { get; set; }
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
            this.SessionName = "Unknown";
            this.Type = 0;
            this.Timestamp = DateTime.UtcNow.Ticks;
            this.Weather = "Unknown";

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
        public long Timestamp { get; set; }
        public byte AmbientTemp { get; set; }
        public byte RoadTemp { get; set; }
        public string Weather { get; set; }
        public List<DriverReport> Connections { get; set; }
        public List<LapReport> Laps { get; set; }
        public List<IncidentReport> Events { get; set; }
    }
}