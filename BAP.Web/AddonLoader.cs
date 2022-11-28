using BAP.Types;
using BAP.Web.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace BAP.Web
{
    public static class AddonLoader
    {

        //return true if a reload is needed.
        public static List<Assembly> AddAllAddonsToDI(this IServiceCollection serviceCollection, string basePath)
        {
            List<Assembly> results = new List<Assembly>();
            try
            {

                var directories = Directory.GetDirectories(basePath);
                List<string> baseDllPlugPaths = new List<string>();
                //search the directories for .deps.json because that is the only DLL we want to load. 
                foreach (var directory in directories)
                {
                    string[] depsFiles = Directory.GetFiles(directory, "*.deps.json");
                    foreach (var depFile in depsFiles)
                    {
                        string assumedDllFileName = $"{directory}\\{Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(depFile))}.dll";
                        if (File.Exists(assumedDllFileName))
                        {
                            baseDllPlugPaths.Add(assumedDllFileName);
                        }
                    }

                }

                foreach (var pluginPath in baseDllPlugPaths)
                {
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    results.Add(pluginAssembly);
                    //var bapGames = GetBapGames(pluginAssembly);
                    //foreach (var bapGame in bapGames)
                    //{
                    //    serviceCollection.AddTransient(typeof(IBapGameDescription), bapGame);
                    //    serviceCollection.AddTransient(bapGame);
                    //}
                    var buttonProviders = GetButtonProviders(pluginAssembly);
                    foreach (var buttonProvider in buttonProviders)
                    {
                        serviceCollection.AddTransient(typeof(IBapButtonProvider), buttonProvider);
                        serviceCollection.AddTransient(buttonProvider);
                    }
                    //var iBapGames = GetIBapGames(pluginAssembly);
                    //foreach (var bapGame in iBapGames)
                    //{
                    //    serviceCollection.AddTransient(bapGame);
                    //}

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
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

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
            foreach (Type type in assembly.GetTypes())
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
                            menuItems.Add(new MenuItemDetail() { DisplayedLabel = menuItemAttribute.DisplayedLabel, MouseOver = menuItemAttribute.MouseOverText, Path = routeAtribute.Template });
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
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IComponent).IsAssignableFrom(type))
                {
                    //This needs to also check for the Custom attribute to indicate what it is for.
                    var gameAttribute = type.GetCustomAttribute<GamePageAttribute>();
                    if (gameAttribute != null)
                    {
                        results.Add(new GameDetail() { Description = gameAttribute.Description, Name = gameAttribute.Name, UniqueId = type?.FullName!, DynamicComponentToLoad = type! });
                    }
                }
            }
            return results;
        }

        static IEnumerable<Type> GetBapGames(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IBapGameDescription).IsAssignableFrom(type))
                {
                    IBapGameDescription? result = Activator.CreateInstance(type) as IBapGameDescription;
                    if (result != null)
                    {
                        count++;
                        yield return type;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements IBapGameDescription in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }

        static IEnumerable<Type> GetIBapGames(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IBapGame).IsAssignableFrom(type))
                {
                    yield return type;
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements IBapGameDescription in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }

        static IEnumerable<Type> GetButtonProviders(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IBapButtonProvider).IsAssignableFrom(type))
                {
                    yield return type;
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements IBapGameDescription in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}
