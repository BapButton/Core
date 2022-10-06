using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BapShared
{
    [MessagePackObject]
    public class ButtonStatus
    {
        [Key(0)]
        public uint BatteryLevel { get; set; }
        [Key(1)]
        public int WifiStrength { get; set; }
        [Key(2)]
        public string IPAddress { get; set; }
        [Key(3)]
        public string VersionId { get; set; }
        public ButtonStatus()
        {
            IPAddress = "";
            VersionId = "";
        }
    }
}
