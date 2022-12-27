using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Db
{
    public class ActiveProvider
    {
        public int ActiveProviderId { get; set; }
        public string ProviderInterfaceName { get; set; } = "";
        public DateTime DateActivated { get; set; }
        public string ProviderUniqueId { get; set; } = "";
        public string ProviderName { get; set; } = "";
    }
}
