using MessagePack;
using System;

namespace BAP.Types
{
    [MessagePackObject]
    public class ButtonPress
    {
        [Key(0)]
        public ulong MillisSinceLight { get; set; }
        [Key(1)]
        public ulong UnixTimeOfPress { get; set; }
        [Key(2)]
        public ulong TimeSinceLightTurnedOff { get; set; }
        [Key(3)]
        public uint MillisOffsetOnPress { get; set; }

        public override string ToString()
        {
            return $"MillisSinceLight of {MillisSinceLight} and TimeSinceLightTurnedOff {TimeSinceLightTurnedOff}";
        }
    }
}
