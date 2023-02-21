using BAP.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace BAP.WebCore
{
    public static class AddonLoader
    {

        //return true if a reload is needed.
        public static List<Assembly> GetAllAddinAssemblies(this IServiceCollection serviceCollection, string basePath)
        {
            List<Assembly> results = new List<Assembly>();
            try
            {
                var directories = Directory.GetDirectories(basePath);
                List<string> baseDllPlugPaths = new List<string>();
                //search the directories for .deps.json because that is the only DLL we want to load. 
                foreach (var directory in directories)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                    string assumedDllFileName = $"{directory}\\{directoryInfo.Name}.dll";
                    if (File.Exists(assumedDllFileName))
                    {
                        baseDllPlugPaths.Add(assumedDllFileName);
                    }
                }

                foreach (var pluginPath in baseDllPlugPaths)
                {
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    results.Add(pluginAssembly);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return results;
        }

        //public static bool AddAllAddonsToDI(this IServiceCollection serviceCollection, string basePath)
        //{
        //    try
        //    {

        //        var directories = Directory.GetDirectories(basePath);
        //        List<string> baseDllPlugPaths = new List<string>();
        //        //search the directories for .deps.json because that is the only DLL we want to load. 
        //        foreach (var directory in directories)
        //        {
        //            string[] depsFiles = Directory.GetFiles(directory, "*.deps.json");
        //            foreach (var depFile in depsFiles)
        //            {
        //                string assumedDllFileName = $"{directory}\\{Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(depFile))}.dll";
        //                if (File.Exists(assumedDllFileName))
        //                {
        //                    baseDllPlugPaths.Add(assumedDllFileName);
        //                }
        //            }

        //        }

        //        foreach (var pluginPath in baseDllPlugPaths)
        //        {
        //            Assembly pluginAssembly = LoadPlugin(pluginPath);
        //            var bapGames = GetBapGames(pluginAssembly);
        //            foreach (var bapGame in bapGames)
        //            {
        //                serviceCollection.AddTransient(typeof(IBapGameDescription), bapGame);
        //                serviceCollection.AddTransient(bapGame);
        //            }
        //            //var gamePages = GetGamePages(pluginAssembly);
        //            //foreach (var gamePage in gamePages)
        //            //{
        //            //    serviceCollection.AddTransient(gamePage);
        //            //}
        //            //var iBapGames = GetIBapGames(pluginAssembly);
        //            //foreach (var bapGame in iBapGames)
        //            //{
        //            //    serviceCollection.AddTransient(bapGame);
        //            //}

        //        }
        //        //If we found a MenuItem then we would need to return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //    return false;
        //}


        private static Assembly LoadPlugin(string relativePath)
        {
            //todo - this seems terrible -there must be a betterway to build the path. 
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(AppContext.BaseDirectory)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            BapPluginLoadContext loadContext = new BapPluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(pluginLocation));
        }
        public static (List<string> routes, List<MenuItemDetail> menuItems, List<TopMenuItemDetail> topMenuItemDetails) GetLoadableComponents(Assembly assembly)
        {
            List<string> routes = new();
            List<MenuItemDetail> menuItems = new();
            List<TopMenuItemDetail> topMenuItemDetails = new();
            foreach (Type type in assembly.GetLoadableTypes())
            {
                if (typeof(IComponent).IsAssignableFrom(type))
                {

                    var routeAtribute = type.GetCustomAttribute<RouteAttribute>();
                    if (routeAtribute != null)
                    {
                        routes.Add(routeAtribute.Template);
                        var menuItemAttribute = type.GetCustomAttribute<MenuItemAttribute>();
                        if (menuItemAttribute != null)
                        {
                            menuItems.Add(new MenuItemDetail() { UniqueId = menuItemAttribute.UniqueId, ShowByDefault = menuItemAttribute.ShowOnMenuByDefault, DisplayedLabel = menuItemAttribute.DisplayedLabel, MouseOver = menuItemAttribute.MouseOverText, Path = routeAtribute.Template });
                        }
                    }
                    var topBarItemAtribute = type.GetCustomAttribute<TopMenuAttribute>();
                    if (topBarItemAtribute != null)
                    {
                        topMenuItemDetails.Add(new TopMenuItemDetail() { DynamicComponentToLoad = type });
                    }
                }
            }
            return (routes, menuItems, topMenuItemDetails);
        }

        public static List<GameDetail> ComponentsWithBapGamePageAttribute(Assembly assembly)
        {
            List<GameDetail> results = new();
            foreach (Type type in assembly.GetLoadableTypes())
            {
                if (typeof(IComponent).IsAssignableFrom(type))
                {
                    //This needs to also check for the Custom attribute to indicate what it is for.
                    var gameAttribute = type.GetCustomAttribute<GamePageAttribute>();
                    if (gameAttribute != null)
                    {
                        results.Add(new GameDetail() { Description = gameAttribute.Description, Name = gameAttribute.Name, UniqueId = gameAttribute.UniqueId, DynamicComponentToLoad = type! });
                    }
                }
            }
            return results;
        }

        public static IEnumerable<Type> GetTypesThatImpementsInterface<T>(Assembly assembly) where T : class
        {
            foreach (Type type in assembly.GetLoadableTypes())
            {
                if (typeof(T).IsAssignableFrom(type) && type.IsClass)
                {

                    yield return type;

                }
            }
        }

        public static IEnumerable<T> GetTypesThatImpementsInterface<T>(T type, Assembly assembly) where T : Type
        {
            foreach (Type t in assembly.GetLoadableTypes())
            {
                if (type.IsAssignableFrom(t) && t.IsClass)
                {

                    yield return (T)t;

                }
            }
        }

        public static IEnumerable<Type> GetInterfacesThatImpementsInterface<T>(Assembly assembly) where T : class
        {
            foreach (Type type in assembly.GetLoadableTypes())
            {
                if (typeof(T).IsAssignableFrom(type) && type.IsInterface)
                {
                    yield return type;

                }
            }
        }

        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null)!;
            }
        }
    }
}
