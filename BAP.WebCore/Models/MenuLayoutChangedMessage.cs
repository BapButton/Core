using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.WebCore.Models
{
    public class MenuLayoutChangedMessage
    {
        public MenuLayoutChangedMessage(bool changed = true)
        {
            Changed = changed;
        }
        bool Changed { get; set; }
    }
}
