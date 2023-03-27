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
                foreach (var directory in directories)
                {
                    Console.WriteLine($"Directory is {directory}");
                    DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                    string assumedDllFileName = Path.Combine(directory, $"{directoryInfo.Name}.dll");
                    Console.WriteLine($"Looking for {assumedDllFileName}");
                    if (File.Exists(assumedDllFileName))
                    {
                        Console.WriteLine($"{assumedDllFileName} exists");
                        baseDllPlugPaths.Add(assumedDllFileName);
                    }
                }

                foreach (var pluginPath in baseDllPlugPaths)
                {
                    Console.WriteLine($"loading plugin {pluginPath}");
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


        private static Assembly LoadPlugin(string fullPath)
        {
            Console.WriteLine($"Loading commands from: {fullPath}");
            BapPluginLoadContext loadContext = new BapPluginLoadContext(fullPath);
            return loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(fullPath));
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
            Console.WriteLine($"Checking {assembly.GetName()} if it has Games");
            List<GameDetail> results = new();
            foreach (Type type in assembly.GetLoadableTypes())
            {
             
                if (typeof(IComponent).IsAssignableFrom(type))
                {
              
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
                Console.WriteLine($"For the Assembly {assembly.GetName()} we had to fall backto getting just some of the types.");
                Console.WriteLine($"The error is {e.Message} with inner exception {e.InnerException?.Message ?? ""}");
                return e.Types.Where(t => t != null)!;
            }
        }
    }
}
