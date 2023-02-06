using BAP.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BAP.WebCore
{
    public static class WebHostStartupMethods
    {
        public static void SetupPages(LoadedAddonHolder loadedAddonHolder, ILogger logger)
        {
            List<(string routeName, string assemblyName)> currentlyAddedRoutes = new();
            foreach (Assembly loadedAssembly in loadedAddonHolder.AllLoadedAssemblies)
            {
                var (routes, menuItems, topMenuItemDetails) = AddonLoader.GetLoadableComponents(loadedAssembly);
                List<(string routeName, string assemblyName)> problemRoutes = new();
                List<(string routeName, string assemblyName)> goodRoutes = new();
                if (routes.Count > 0)
                {

                    foreach (var page in routes)
                    {
                        (string routeName, string assemblyName) matchingRoute = currentlyAddedRoutes.FirstOrDefault(t => t.routeName.Equals(page, StringComparison.OrdinalIgnoreCase));
                        if (matchingRoute != default)
                        {
                            problemRoutes.Add(matchingRoute);
                        }
                        else
                        {
                            goodRoutes.Add((page, loadedAssembly.FullName));
                        }
                    }
                    if (problemRoutes.Count == 0)
                    {
                        currentlyAddedRoutes.AddRange(goodRoutes);
                        loadedAddonHolder.AssembliesWithPages.Add(loadedAssembly);
                    }
                    else
                    {
                        foreach (var item in problemRoutes)
                        {
                            logger.LogError($"Could not load routes for Assembly {loadedAssembly.FullName} because it matches route {item.routeName} which is already prepared for loading from {item.assemblyName}");
                        }

                    }
                }

                if (problemRoutes.Count == 0)
                {
                    loadedAddonHolder.MainMenuItems.AddRange(menuItems);
                    loadedAddonHolder.TopBarItems.AddRange(topMenuItemDetails);
                }

            }
        }
    }
}
