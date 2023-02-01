using BAP.Db;
using BAP.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.WebCore
{
    public static class CoreDiServices
    {
        public static void AddAllAddonsAndRequiredDiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BapSettings>(configuration.GetSection("BAP"));
            var bapSettings = configuration.GetSection("BAP").Get<BapSettings>();
            if (string.IsNullOrEmpty(bapSettings?.DBConnectionString))
            {
                services.AddDbContextFactory<ButtonContext>(options => options.UseInMemoryDatabase(new Guid().ToString()));
            }
            else
            {
                services.AddDbContextFactory<ButtonContext>(options => options.UseMySql(bapSettings.DBConnectionString, MySqlServerVersion.LatestSupportedServerVersion));
            }

            services.AddTransient(p => p.GetRequiredService<IDbContextFactory<ButtonContext>>().CreateDbContext());
            services.AddTransient<DbAccessor>();
            services.AddTransient<PhysicalFileMaintainer>();
            //services.AddSingleton<AnimationController>(); ;
            services.AddMessagePipe();
            services.AddTransient<IGameDataSaver, DefaultGameDataSaver>();
            services.AddHostedService<BapProviderInitializer>();
            if (bapSettings != null)
            {
                PhysicalFileMaintainer physicalFileMaintainer = new PhysicalFileMaintainer(new BapSettingsFakeOptionSnapshot(bapSettings));
                physicalFileMaintainer.CleanUpPackages();

            }

            LoadedAddonHolder addonHolder = new();
            addonHolder.AllAddonAssemblies = AddonLoader.GetAllAddinAssemblies(services, bapSettings?.AddonSaveLocation ?? "");
            addonHolder.AllCompiledAssembies = AssemblyScanner.GetAllDependentAssemblies();
            foreach (var assembly in addonHolder.AllLoadedAssemblies)
            {
                var providerInterfaces = AddonLoader.GetInterfacesThatImpementsInterface<IBapProvider>(assembly);
                foreach (var providerInterface in providerInterfaces)
                {
                    if (Attribute.IsDefined(providerInterface, typeof(BapProviderInterfaceAttribute)))
                    {
                        var parentInfo = (BapProviderInterfaceAttribute?)Attribute.GetCustomAttribute(providerInterface, typeof(BapProviderInterfaceAttribute));

                        addonHolder.BapProviders.Add(new()
                        {
                            ProviderInterfaceType = providerInterface,
                            Description = parentInfo?.Description ?? providerInterface?.FullName ?? "",
                            Name = parentInfo?.Name ?? providerInterface?.Name ?? "",
                            AllowMultipleInstances = parentInfo?.AllowMultipleInstances ?? false
                        });

                    }

                }
            }
            foreach (var assembly in addonHolder.AllLoadedAssemblies)
            {
                foreach (var bapProviderInterface in addonHolder.BapProviders)
                {
                    var providers = AddonLoader.GetTypesThatImpementsInterface(bapProviderInterface.ProviderInterfaceType, assembly);
                    foreach (var provider in providers)
                    {
                        var providerInfo = (BapProviderAttribute?)Attribute.GetCustomAttribute(provider, typeof(BapProviderAttribute));

                        bapProviderInterface.Providers.Add(new()
                        {
                            BapProviderType = provider,
                            Description = providerInfo?.Description ?? provider?.Name ?? string.Empty,
                            Name = providerInfo?.Name ?? provider?.Name ?? string.Empty,
                            UniqueId = providerInfo?.UniqueId ?? provider?.FullName ?? string.Empty,


                        });
                    }
                }
                var messageSenders = AddonLoader.GetTypesThatImpementsInterface<IBapMessageSender>(assembly);
                foreach (var messageSender in messageSenders)
                {
                    services.AddTransient(typeof(IBapMessageSender), messageSender);
                    services.AddTransient(messageSender);
                }

                var gamePages = AddonLoader.ComponentsWithBapGamePageAttribute(assembly);
                addonHolder.AllGames.AddRange(gamePages);
                var diSetups = AddonLoader.GetTypesThatImpementsInterface<IDependencyInjectionSetup>(assembly);
                foreach (var diSetup in diSetups)
                {
                    if (diSetup.IsClass)
                    {
                        IDependencyInjectionSetup? dependencyInjectionSetup = (IDependencyInjectionSetup?)Activator.CreateInstance(diSetup);
                        if (dependencyInjectionSetup != null)
                        {
                            dependencyInjectionSetup.AddItemsToDi(services);
                        }

                    }

                }

            }
            //This is a non dynamic component. But it's what I got for now.
            services.AddTransient<IGameDataSaver, DefaultGameDataSaver>();
            //Todo - I think gamehandler needs to go away. But this gets some stuff to actually build for now. 
            services.AddTransient<IGameHandler, DefaultGameHandler>();

            services.AddSingleton(addonHolder);
            var factory = new TempButtonContextFactory(bapSettings?.DBConnectionString ?? "");

            DbAccessor dba = new DbAccessor(factory);
            foreach (var provider in addonHolder.BapProviders)
            {
                Type? providerType = null;
                var recentProvider = dba.GetRecentlyActiveProvider(provider.ProviderInterfaceType.FullName).FirstOrDefault();
                if (recentProvider != null)
                {
                    providerType = provider.Providers.FirstOrDefault(t => t.UniqueId == recentProvider)?.BapProviderType;
                }
                if (providerType == null)
                {
                    providerType = provider.Providers.FirstOrDefault(t => t.Name.Contains("Default", StringComparison.OrdinalIgnoreCase))?.BapProviderType;
                }
                if (providerType == null)
                {
                    providerType = provider.Providers.FirstOrDefault()?.BapProviderType;
                }
                if (providerType != null)
                {
                    services.AddSingleton(provider.ProviderInterfaceType, providerType);
                    provider.Providers.Where(t => t.BapProviderType == providerType).First().IsCurrentlySelected = true;
                }

            }
        }
    }
}
