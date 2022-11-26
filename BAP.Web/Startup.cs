using BAP.Db;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Targets;
using BAP.Web.Models;
using BAP.Web.TTS;
using Scrutor;
using MudBlazor.Services;
using BAP.PrimaryHandlers;
using System.Reflection;
using System.Linq;

namespace BAP.Web
{
    public class Startup
    {
        const string cacheMaxAge = "7200";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //this is tied to UseMvcWithDefaultRoute - so I need to figure out why I am using this.
            services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddRazorPages();
            services.AddServerSideBlazor();
            //services.AddSingleton<IGameManager, CoreGameManager>();
            services.AddSingleton<AudioManager>();
            services.AddSingleton<IGameHandler, DefaultGameHandler>();
            services.AddSingleton<ILoadablePageHandler, DefaultLoadablePageHandler>();
            services.AddSingleton<ILayoutHandler, DefaultLayoutHandler>();
            services.AddSingleton<IKeyboardHandler, DefaultKeyboardHandler>();
            services.AddSingleton<ControlHandler>();
            services.AddHostedService<ConnectionCoreHostedService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient<ITtsService, TtsService>(client => client.BaseAddress = new Uri("http://localhost:5002/api/"));
            services.AddTransient<IValidator<FileUpload>, FileUploadValidator>();
            services.AddDbContextFactory<ButtonContext>();
            services.AddTransient(p => p.GetRequiredService<IDbContextFactory<ButtonContext>>().CreateDbContext());
            services.AddTransient<DbAccessor>();
            services.AddSingleton<AnimationController>();
            services.AddSingleton<LoadedAddonHolder>();
            services.AddMessagePipe();
            services.AddTransient<IGameDataSaver, DefaultGameDataSaver>();
            //services.Scan(scan => scan.FromApplicationDependencies()
            //        .AddClasses(t => { t.AssignableTo<IGameDataSaver>(); })
            //          .AsImplementedInterfaces()
            //          .WithTransientLifetime());
            services.Scan(scan => scan.FromApplicationDependencies()
                    .AddClasses(t => { t.AssignableTo<IBapMessageSender>(); })
                      .AsImplementedInterfaces()
                      .WithTransientLifetime());
            services.Scan(scan => scan.FromApplicationDependencies()
                    .AddClasses(t => { t.AssignableTo<IBapButtonProvider>(); })
                      .AsImplementedInterfaces()
                      .WithTransientLifetime());
            services.Scan(scan => scan.FromApplicationDependencies()
                    .AddClasses(t => { t.AssignableTo<IBapGame>(); })
                      .AsSelf()
                      .WithTransientLifetime());
            //Todo - this seems totally wrong. Why would a GameDescription be a singleton. 
            services.Scan(scan => scan.FromApplicationDependencies()
                    .AddClasses(t => { t.AssignableTo<IBapGameDescription>(); })
                      .AsImplementedInterfaces().WithSingletonLifetime());
            services.Scan(scan => scan.FromApplicationDependencies()
                    .AddClasses(t => { t.AssignableTo<IBapGameDescription>(); })
                      .AsSelf().WithSingletonLifetime());
            services.Scan(scan => scan.FromApplicationDependencies()
                    .AddClasses(t => { t.AssignableTo<IBapKeyboardProvider>(); })
                        .AsSelf().WithTransientLifetime());
            services.Scan(scan => scan.FromApplicationDependencies()
                    .AddClasses(t => { t.AssignableTo<IMainMenuItem>(); })
                        .AsSelf().WithSingletonLifetime());
            services.Scan(scan => scan.FromApplicationDependencies()
                    .AddClasses(t => { t.AssignableTo<IMainMenuItem>(); })
                         .AsImplementedInterfaces().WithSingletonLifetime());

            //This makes the app not start. I don't have any idea why.    
            services.Scan(scan => scan.FromApplicationDependencies()
                .AddClasses(t => { t.AssignableTo<IBapKeyboardProvider>(); })
                  .AsImplementedInterfaces().WithTransientLifetime());
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

            LoadedAddonHolder addonHolder = new();
            addonHolder.AllLoadedAssemblies = AddonLoader.AddAllAddonsToDI(services, "C:\\Users\\nick.gelotte\\source\\repos\\BapButton\\Core\\BAP.TestUtilities\\bin\\Debug");

            services.AddSingleton<LoadedAddonHolder>(addonHolder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, LoadedAddonHolder loadedAddonHolder, ILogger<Startup> logger)
        {
            List<(string routeName, string assemblyName)> currentlyAddedRoutes = new();
            foreach (Assembly loadedAssembly in loadedAddonHolder.AllLoadedAssemblies)
            {
                List<string> routes = AddonLoader.PagesWithRouting(loadedAssembly);
                if (routes.Count > 0)
                {
                    List<(string routeName, string assemblyName)> problemRoutes = new();
                    List<(string routeName, string assemblyName)> goodRoutes = new();
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

            }


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    OnPrepareResponse = ctx =>
            //    {
            //        ctx.Context.Response.Headers.Append(
            //             "Cache-Control", $"public, max-age={cacheMaxAge}");
            //    }
            //});

            app.UseRouting();
            app.UseMvcWithDefaultRoute();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
            MethodCallTarget target = new MethodCallTarget("LiveLogger", (logEvent, parms) => LiveLogger.RecordNewLogMessage(logEvent.LoggerName, logEvent.Level, logEvent.FormattedMessage));
            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, NLog.LogLevel.Trace);
            //NLog.Config.SimpleConfigurator.ConfigureForConsoleLogging();
        }
    }
}
