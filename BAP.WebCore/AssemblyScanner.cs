using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace BAP.WebCore
{
    public class AssemblyScanner
    {
        private static List<string> BuiltinNamesToIgnore = new List<string>() { "System", "AspNetCore", "Microsoft", "mscorlib", "netstandard", "WindowsBase","MessagePack","MessagePipe","MQTTnet", "MudBlazor", "MySqlConnector", "NLog", "Pomelo", "SixLabors.ImageSharp","Nuget" };

        public static List<Assembly> GetAllDependentAssemblies()
        {
            var assembliesByName = DependencyContext.Default.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default));

            return LoadAssemblies(assembliesByName).ToList();



        }
        private static IEnumerable<Assembly> LoadAssemblies(IEnumerable<AssemblyName> assemblyNames)
        {
            var assemblies = new List<Assembly>();

            foreach (var assemblyName in assemblyNames)
            {
                if (!BuiltinNamesToIgnore.Any(t => assemblyName.FullName.StartsWith(t, StringComparison.OrdinalIgnoreCase)))
                    try
                    {
                        // Try to load the referenced assembly...
                        assemblies.Add(Assembly.Load(assemblyName));
                    }
                    catch
                    {
                        // Failed to load assembly. Skip it.
                    }
            }

            return assemblies;
        }
    }
}
