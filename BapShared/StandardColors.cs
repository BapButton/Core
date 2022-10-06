using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace BapShared
{
    public class StandardColorPalettes
    {
        public static List<BapColor> Default { get; } = new()
        {
            new(55, 94, 151),
            new(91, 201, 172),
            new(230, 215, 42),
            new(241, 141, 158),
            new(63, 104, 28),
            new(251, 101, 66),
            new(141, 35, 15),
            new(241, 241, 241),
            new(142, 186, 67),
            new(240, 129, 15),
            new(125, 42, 232),
        };
    }
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
