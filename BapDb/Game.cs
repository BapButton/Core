using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BapDb
{
    public class GameFavorite
    {
        public int GameFavoriteId { get; set; }
        [StringLength(40)]
        public string GameUniqueId { get; set; } = "";
        public bool IsFavorite { get; set; }
    }
    public class GamePlayLog
    {
        public int GamePlayLogId { get; set; }
        [StringLength(40)]
        public string GameUniqueId { get; set; } = "";
        public DateTime DateGameSelectedUTC { get; set; }
    }
}
