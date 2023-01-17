﻿using BAP.Db;
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
            foreach (var provider in addonHolder.BapProviders)
            {
                var stuff = provider.Providers.OrderBy(t => t.Name).First().BapProviderType;
                services.AddTransient(provider.ProviderInterfaceType, stuff);
            }
        }
    }
}
