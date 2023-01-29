using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Db
{
    public class ActiveProvider
    {
        public int ActiveProviderId { get; set; }
        public string ProviderInterfaceFullName { get; set; } = "";
        public DateTime DateActivated { get; set; }
        [Required]
        [MaxLength(255)]
        public string ProviderUniqueId { get; set; } = "";
        [Required]
        [MaxLength(255)]
        public string ProviderName { get; set; } = "";
        [Required]
        [MaxLength(255)]
        public string ProviderDescription { get; set; } = "";
    }
}
