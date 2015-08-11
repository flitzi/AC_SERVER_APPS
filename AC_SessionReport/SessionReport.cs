using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AC_SessionReport
{
    [DataContract]
    public struct Single3
    {
        [DataMember]
        public float X { get; set; }
        [DataMember]
        public float Y { get; set; }
        [DataMember]
        public float Z { get; set; }

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

    [DataContract]
    public class DriverReport
    {
        public DriverReport()
        {
            this.StartPosNs = -1.0;
            this.LastPosNs = -1.0;
            this.ConnectedTimestamp = -1;
        }

        private const double MaxSpeed = 1000; // km/h
        private const double MinSpeed = 5; // km/h

        [DataMember]
        public int ConnectionId { get; set; }
        [DataMember]
        public long ConnectedTimestamp { get; set; }
        [DataMember]
        public long DisconnectedTimestamp { get; set; }
        [DataMember]
        public string SteamId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Team { get; set; } // currently not set
        [DataMember]
        public byte CarId { get; set; }
        [DataMember]
        public string CarModel { get; set; }
        [DataMember]
        public string CarSkin { get; set; }
        [DataMember]
        public short BallastKG { get; set; } // currently not set
        [DataMember]
        public int BestLap { get; set; }
        [DataMember]
        public int TotalTime { get; set; }
        [DataMember]
        public short LapCount { get; set; }
        [DataMember]
        public short StartPosition { get; set; } // only set for race session
        [DataMember]
        public short Position { get; set; }
        [DataMember]
        public string Gap { get; set; }
        [DataMember]
        public int Incidents { get; set; }
        [DataMember]
        public double Distance { get; set; }
        [DataMember]
        public double TopSpeed { get; set; } // km/h
        [DataMember]
        public double StartPosNs { get; set; }
        [DataMember]
        public double LastPosNs { get; set; }
        [DataMember]
        public bool IsAdmin { get; set; }

        private int lastTime = -1;
        private Single3 lastPos;

        public void AddDistance(Single3 pos, Single3 vel, double s)
        {
            if (StartPosNs == -1.0)
            {
                StartPosNs = s > 0.5 ? s - 1.0 : s;
            }

            double currentSpeed = vel.Length() * 3.6;
            if (currentSpeed < MaxSpeed && currentSpeed > TopSpeed)
            {
                this.TopSpeed = currentSpeed;
            }

            int currTime = Environment.TickCount;
            if (this.lastTime > 0)
            {
                double d = (pos - lastPos).Length();

                double speed = d / (currTime - this.lastTime) / 1000 * 3.6;

                if (speed < MaxSpeed && currentSpeed > MinSpeed)
                {
                    this.Distance += d;
                    this.LastPosNs = s;
                }
            }
            this.lastPos = pos;

            this.lastTime = currTime;
        }
    }

    [DataContract]
    public class LapReport
    {
        [DataMember]
        public int ConnectionId { get; set; }
        [DataMember]
        public long Timestamp { get; set; }
        [DataMember]
        public int LapTime { get; set; }
        [DataMember]
        public short LapNo { get; set; }
        [DataMember]
        public short Position { get; set; }
        [DataMember]
        public short Cuts { get; set; }
        [DataMember]
        public float Grip { get; set; }
    }

    [DataContract]
    public class IncidentReport
    {
        [DataMember]
        public byte Type { get; set; }
        [DataMember]
        public long Timestamp { get; set; }
        [DataMember]
        public int ConnectionId1 { get; set; }
        [DataMember]
        public int ConnectionId2 { get; set; }
        [DataMember]
        public float ImpactSpeed { get; set; }
        [DataMember]
        public Single3 WorldPosition { get; set; }
        [DataMember]
        public Single3 RelPosition { get; set; }
    }

    [DataContract]
    public class SessionReport
    {
        public const string Version = "1.2.0";

        public SessionReport()
        {
            this.ReportVersion = Version;
            this.ProtocolVersion = -1;
            this.SessionName = "Unknown";
            this.Type = 0;
            this.Timestamp = DateTime.UtcNow.Ticks;
            this.Weather = "Unknown";
            this.Connections = new List<DriverReport>();
            this.Laps = new List<LapReport>();
            this.Events = new List<IncidentReport>();
        }

        [DataMember]
        public string ReportVersion { get; set; }
        [DataMember]
        public short ProtocolVersion { get; set; }
        [DataMember]
        public string ServerName { get; set; }
        [DataMember]
        public string TrackName { get; set; }
        [DataMember]
        public string TrackConfig { get; set; }
        [DataMember]
        public string SessionName { get; set; }
        [DataMember]
        public byte Type { get; set; }
        [DataMember]
        public int Time { get; set; }
        [DataMember]
        public short RaceLaps { get; set; }
        [DataMember]
        public int WaitTime { get; set; }
        [DataMember]
        public long Timestamp { get; set; }
        [DataMember]
        public byte AmbientTemp { get; set; }
        [DataMember]
        public byte RoadTemp { get; set; }
        [DataMember]
        public string Weather { get; set; }
        [DataMember]
        public List<DriverReport> Connections { get; set; }
        [DataMember]
        public List<LapReport> Laps { get; set; }
        [DataMember]
        public List<IncidentReport> Events { get; set; }
    }
}