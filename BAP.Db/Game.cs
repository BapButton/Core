using System;
using System.ComponentModel.DataAnnotations;

namespace BAP.Db
{
    public class GameFavorite
    {
        public int GameFavoriteId { get; set; }
        [Required]
        [MaxLength(255)]
        public string GameUniqueId { get; set; } = "";
        public bool IsFavorite { get; set; }
    }
    public class GamePlayLog
    {
        public int GamePlayLogId { get; set; }
        [Required]
        [MaxLength(255)]
        public string GameUniqueId { get; set; } = "";
        public DateTime DateGameSelectedUTC { get; set; }
    }
    public class MenuItemStatus
    {

        public int MenuItemStatusId { get; set; }
        [Required]
        [MaxLength(255)]
        public string MenuItemUniqueId { get; set; } = "";
        public bool ShowInMainMenu { get; set; }
        public int Order { get; set; }
    }
}
