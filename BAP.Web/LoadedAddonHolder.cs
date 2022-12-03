using System.Reflection;

namespace BAP.Web
{
    public class LoadedAddonHolder
    {
        public List<GameDetail> AllGames { get; set; } = new();
        public List<MenuItemDetail> MainMenuItems { get; set; } = new();
        public List<TopMenuItemDetail> TopBarItems { get; set; } = new();
        public List<Assembly> AssembliesWithPages { get; set; } = new();
        public List<Assembly> AllAddonAssemblies { get; set; } = new();
        public List<Assembly> AllCompiledAssembies { get; set; } = new();
        public IEnumerable<Assembly> AllLoadedAssemblies
        {
            get
            {
                return AllCompiledAssembies.Concat(AllAddonAssemblies);
            }
        }
    }

    public class GameDetail : IMainAreaItem
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string UniqueId { get; set; } = "";
        public Type DynamicComponentToLoad { get; set; } = default!;
    }

    public class MenuItemDetail
    {
        public string DisplayedLabel { get; set; } = "";
        public string MouseOver { get; set; } = "";
        public string UniqueId { get; set; } = "";
        public bool ShowByDefault { get; set; } = false;
        public string Path { get; set; } = "";
    }

    public class TopMenuItemDetail
    {
        public Type DynamicComponentToLoad { get; set; } = default!;
    }
}
