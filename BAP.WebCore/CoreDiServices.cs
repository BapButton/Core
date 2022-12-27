using BAP.Db;
using BAP.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.WebCore
{
    public static class CoreDiServices
    {
        public static void AddAllAddonsAndRequiredDiServices(this IServiceCollection services)
        {
            services.AddDbContextFactory<ButtonContext>();
            services.AddTransient(p => p.GetRequiredService<IDbContextFactory<ButtonContext>>().CreateDbContext());
            services.AddTransient<DbAccessor>();
            //services.AddSingleton<AnimationController>(); ;
            services.AddMessagePipe();
            services.AddTransient<IGameDataSaver, DefaultGameDataSaver>();



            LoadedAddonHolder addonHolder = new();
            addonHolder.AllAddonAssemblies = AddonLoader.GetAllAddinAssemblies(services, "C:\\Users\\nick.gelotte\\source\\repos\\BapButton\\Core\\BAP.TestUtilities\\bin\\Debug");
            addonHolder.AllCompiledAssembies = AssemblyScanner.GetAllDependentAssemblies();
            foreach (var assembly in addonHolder.AllLoadedAssemblies)
            {
                var buttonProviders = AddonLoader.GetTypesThatImpementsInterface<IBapProvider>(assembly);
                foreach (var buttonProvider in buttonProviders)
                {
                    services.AddTransient(typeof(IButtonProvider), buttonProvider);
                    services.AddTransient(buttonProvider);
                }
                var keyboardProviders = AddonLoader.GetTypesThatImpementsInterface<IKeyboardHandler>(assembly);
                foreach (var keyboardProvider in keyboardProviders)
                {
                    services.AddTransient(typeof(IKeyboardHandler), keyboardProvider);
                    services.AddTransient(keyboardProvider);
                }
                var messageSenders = AddonLoader.GetTypesThatImpementsInterface<IBapMessageSender>(assembly);
                foreach (var messageSender in messageSenders)
                {
                    services.AddTransient(typeof(IBapMessageSender), messageSender);
                    services.AddTransient(messageSender);
                }
                var diSetups = AddonLoader.GetTypesThatImpementsInterface<IDependencyInjectionSetup>(assembly);
                foreach (var diSetup in diSetups)
                {
                    if (diSetup.IsClass)
                    {
                        IDependencyInjectionSetup? dependencyInjectionSetup = (IDependencyInjectionSetup)Activator.CreateInstance(diSetup);
                        if (dependencyInjectionSetup != null)
                        {
                            dependencyInjectionSetup.AddItemsToDi(services);
                        }

                    }

                }

            }
            services.AddSingleton(addonHolder);
        }
    }
}
