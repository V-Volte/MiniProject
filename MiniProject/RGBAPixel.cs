using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject
{
    class RGBAPixel
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public SByte Alpha { get; set; }

        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}, {3}]", Red.ToString("x"), Green.ToString("x"), Blue.ToString("x"), Alpha.ToString("x"));
        }

        public RGBAPixel()
        {
            Red = 0;
            Green = 0;
            Blue = 0;
            Alpha = 1;
        }

        public RGBAPixel(byte[] colours)
        {

            if (colours.Length == 4)
            {
                Alpha = (sbyte)colours[^1];
                Red = colours[^2];
                Green = colours[^3];
                Blue = colours[^4];
            }

            else
            {
                Red = colours[^1];

                if (colours.Length == 3)
                {
                    Green = colours[^2];
                    Blue = colours[^3];
                }
            }
        }

        public byte[] GetArray(int cdepth)
        {
            if (cdepth == 1) return new byte[] { Red };
            if (cdepth == 3) return new byte[] { Blue, Green, Red };
            if (cdepth == 4) return new byte[] {Blue, Green, Red, (byte) (Alpha + (byte) 128) };
            else throw new Exception("Impossible colour depth.");
        }
    }
}
