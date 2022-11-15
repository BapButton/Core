using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BapShared
{
    
    public class Verify
    {
        [Key(0)]
        public bool MsgId { get; set; }
    }
}
