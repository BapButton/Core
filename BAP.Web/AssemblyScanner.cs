using Microsoft.Extensions.DependencyModel;
using Scrutor;
using System.Reflection;

namespace BAP.Web
{
    public class AssemblyScanner
    {
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
