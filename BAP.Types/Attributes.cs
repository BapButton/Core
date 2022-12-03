using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BAP.Types
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class GamePageAttribute : Attribute
    {
        public string Description { get; set; } = "";
        public string Name { get; set; } = "";
        public string UniqueId { get; set; } = "";
        public GamePageAttribute(string name, string description, string uniqueId = "")
        {
            Name = name;
            Description = description;
            UniqueId = string.IsNullOrEmpty(uniqueId) ? this.GetType().FullName ?? "" : uniqueId;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MenuItemAttribute : Attribute
    {
        public string MouseOverText { get; set; } = "";
        public string DisplayedLabel { get; set; } = "";
        public bool ShowOnMenuByDefault { get; set; } = false;
        public string UniqueId { get; set; } = "";
        public MenuItemAttribute(string displayedLabel, string mouseOverText, bool showOnMenuByDefault = false, string uniqueId = "")
        {
            DisplayedLabel = displayedLabel;
            MouseOverText = mouseOverText;
            ShowOnMenuByDefault = showOnMenuByDefault;
            UniqueId = string.IsNullOrEmpty(uniqueId) ? this.GetType().FullName ?? "" : uniqueId;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TopMenuAttribute : Attribute
    {
    }
}
