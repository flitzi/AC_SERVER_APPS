using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_ServerStarter
{
    public class RaceSession
    {
        public readonly string Track, Layout;
        public readonly int Laps;

        public RaceSession(string track, string layout, int laps)
        {
            this.Track = track;
            this.Layout = layout;
            this.Laps = laps;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Layout))
            {
                return this.Track + " " + this.Laps + " laps";
            }
            return this.Track + "," + this.Layout + " " + this.Laps + " laps";
        }
    }
}
