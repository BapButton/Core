using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Db
{
    public class GameStorage
    {
        public int GameStorageId { get; set; }
        public string GameUniqueId { get; set; } = "";
        [MaxLength(120)]
        public string Key { get; set; } = "";
        public string Data { get; set; } = "";
    }
}
