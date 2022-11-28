using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class GamePageAttribute : Attribute
    {
        public string Description { get; set; } = "";
        public string Name { get; set; } = "";
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MenuItemAttribute : Attribute
    {
        public string MouseOverText { get; set; } = "";
        public string DisplayedLabel { get; set; } = "";
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TopMenuAttribute : Attribute
    {
    }
}
