using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using BAP.Types;

namespace BAP.Helpers
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
}
