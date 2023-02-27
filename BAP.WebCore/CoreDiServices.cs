using BAP.Db;
using BAP.Helpers;
using BAP.Web;
using BAP.WebCore.TTS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using NLog.Targets;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using MudBlazor.Charts;
using Microsoft.Extensions.Logging;
using System.Reflection.Emit;

namespace BAP.WebCore
{
    public static class CoreDiServices
    {
        private static LogLevel NlogLevelConvertor(NLog.LogLevel level)
        {
            LogLevel logLevel = LogLevel.Trace;
            if (level == NLog.LogLevel.Info)
            {
                logLevel = LogLevel.Information;
            }
            else if (level == NLog.LogLevel.Debug)
            {
                logLevel = LogLevel.Debug;
            }
            else if (level == NLog.LogLevel.Fatal)
            {
                logLevel = LogLevel.Error;
            }
            else if (level == NLog.LogLevel.Trace)
            {
                logLevel = LogLevel.Trace;
            }
            return logLevel;
        }

        public static void SetupPostDIBapServices(this IHost app)
        {
            var logProvider = app.Services.GetRequiredService<ILogProvider>();

            MethodCallTarget target = new MethodCallTarget("LiveLogger", (logEvent, parms) => logProvider.RecordNewLogMessage(logEvent.LoggerName, NlogLevelConvertor(logEvent.Level), logEvent.FormattedMessage));
            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, NLog.LogLevel.Trace);


            app.Services.GetRequiredService<LoadedAddonHolder>();
            var logger = app.Services.GetRequiredService<ILogger<BapPluginLoadContext>>();
            WebHostStartupMethods.SetupPages(app.Services.GetRequiredService<LoadedAddonHolder>(), logger);
        }


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
            services.AddMessagePipe();
            services.AddTransient<IGameDataSaver, DefaultGameDataSaver>();
            services.AddHostedService<BapProviderInitializer>();
            services.AddHttpClient<ITtsService, TtsService>(client => client.BaseAddress = new Uri("http://localhost:5002/api/"));
            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

                config.SnackbarConfiguration.PreventDuplicates = true;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 10000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });
            services.AddLogging();


            if (bapSettings != null)
            {
                //todo - This is most likely a garbage way to make a logger;
                PhysicalFileMaintainer physicalFileMaintainer = new PhysicalFileMaintainer(new BapSettingsFakeOptionSnapshot(bapSettings), new Logger<PhysicalFileMaintainer>(new LoggerFactory()));

                AsyncHelpers.RunSync(() => physicalFileMaintainer.CleanUpPackages());

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
