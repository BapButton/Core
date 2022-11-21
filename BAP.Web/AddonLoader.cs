using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace BAP.Web
{
    public static class AddonLoader
    {

        //return true if a reload is needed.
        public static bool AddAllAddonsToDI(this IServiceCollection serviceCollection, string basePath)
        {
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
                    var bapGames = GetBapGames(pluginAssembly);
                    foreach (var bapGame in bapGames)
                    {
                        serviceCollection.AddTransient(typeof(IBapGameDescription), bapGame);
                    }
                    var gamePages = GetGamePages(pluginAssembly);
                    foreach (var gamePage in gamePages)
                    {
                        serviceCollection.AddTransient(gamePage);
                    }
                    var iBapGames = GetIBapGames(pluginAssembly);
                    foreach (var bapGame in iBapGames)
                    {
                        serviceCollection.AddTransient(bapGame);
                    }

                }
                //If we found a MenuItem then we would need to return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }


        private static Assembly LoadPlugin(string relativePath)
        {
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

        static IEnumerable<Type> GetGamePages(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IGamePage).IsAssignableFrom(type))
                {
                    IGamePage? result = Activator.CreateInstance(type) as IGamePage;
                    if (result != null)
                    {
                        count++;
                        yield return type;
                    }
                    else
                    {
                        Console.WriteLine("Well we could not activate it");
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
    }
}
