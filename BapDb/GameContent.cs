using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BapDb
{
    public class GameStorage
    {
        public int GameStorageId { get; set; }
        public string GameUniqueId { get; set; } = "";
        public string Data { get; set; } = "";
    }
}
