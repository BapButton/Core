using System.Reflection;

namespace BAP.Web
{
    public class LoadedAddonHolder
    {
        public List<IBapGameDescription> AllGames { get; set; } = new();
        public List<IMainMenuItem> MainMenuItems { get; set; } = new();
        public List<ITopBarItem> TopBarItmes { get; set; } = new();
        public List<Assembly> AssembliesWithPages { get; set; } = new();
        public List<Assembly> AllLoadedAssemblies { get; set; } = new();
    }
}
