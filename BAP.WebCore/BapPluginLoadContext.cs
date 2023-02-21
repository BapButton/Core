using System.Reflection;
using System.Runtime.Loader;

namespace BAP.WebCore
{
    public class BapPluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public BapPluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name == "BAP.Types" || assemblyName.Name == "BAP.Db")
            {
                return null;
            }
            string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        //protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        //{
        //    string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        //    if (libraryPath != null)
        //    {
        //        return LoadUnmanagedDllFromPath(libraryPath);
        //    }

        //    return IntPtr.Zero;
        //}
    }
}
