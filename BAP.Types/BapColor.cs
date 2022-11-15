using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
    public record BapColor
    {
        public byte Red { get; init; }
        public byte Green { get; init; }
        public byte Blue { get; init; }
        public BapColor(int Red, int Green, int Blue) : this((ushort)Red, (ushort)Green, (ushort)Blue)
        {

        }
        public BapColor(ushort red, ushort green, ushort blue)
        {
            Red = (byte)red;
            Green = (byte)green;
            Blue = (byte)blue;
        }

        public ulong LongColor
        {
            get
            {
                return (ulong)(Red << 16 | Green << 8 | Blue);
            }
        }
    }
}
