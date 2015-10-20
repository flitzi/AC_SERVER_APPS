using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AC_TrackCycle
{
    /// <summary>
    /// AHSV struct. Alpha, Hue, Saturation, Value color.
    /// </summary>
    public struct AHSV
    {
        /// <summary>
        /// The alpha. Between 0 and 255.
        /// </summary>
        public byte Alpha;

        /// <summary>
        /// The hue. Between 0 and 360.
        /// </summary>
        public float Hue;

        /// <summary>
        /// The saturation. Between 0 and 1.
        /// </summary>
        public float Saturation;

        /// <summary>
        /// The value. Between 0 and 1.
        /// </summary>
        public float Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="AHSV"/> struct.
        /// </summary>
        /// <param name="A">The alpha. Between 0 and 255.</param>
        /// <param name="H">The hue. Between 0 and 360.</param>
        /// <param name="S">The saturation. Between 0 and 1.</param>
        /// <param name="V">The value. Between 0 and 1.</param>
        public AHSV(byte A, float H, float S, float V)
        {
            this.Hue = H;
            this.Saturation = S;
            this.Value = V;
            this.Alpha = A;
        }

        /// <summary>
        /// Converts the AHSV to the ARGB.
        /// </summary>
        /// <returns>The ARGB.</returns>
        public Color ToColor()
        {
            double r = 0;
            double g = 0;
            double b = 0;

            if (this.Saturation == 0)
            {
                r = g = b = this.Value;
            }
            else
            {
                // the color wheel consists of 6 sectors. Figure out which sector you're in.
                double sectorPos = this.Hue / 60.0;
                int sectorNumber = (int)(Math.Floor(sectorPos));
                // get the fractional part of the sector
                double fractionalSector = sectorPos - sectorNumber;

                // calculate values for the three axes of the color.
                double p = this.Value * (1.0 - this.Saturation);
                double q = this.Value * (1.0 - (this.Saturation * fractionalSector));
                double t = this.Value * (1.0 - (this.Saturation * (1 - fractionalSector)));

                // assign the fractional colors to r, g, and b based on the sector
                // the angle is in.
                switch (sectorNumber)
                {
                    case 0:
                        r = this.Value;
                        g = t;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = this.Value;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = this.Value;
                        b = t;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = this.Value;
                        break;
                    case 4:
                        r = t;
                        g = p;
                        b = this.Value;
                        break;
                    case 5:
                        r = this.Value;
                        g = p;
                        b = q;
                        break;
                }
            }

            return Color.FromArgb(this.Alpha, (byte)Math.Round(255 * r), (byte)Math.Round(255 * g), (byte)Math.Round(255 * b));
        }
    }
}