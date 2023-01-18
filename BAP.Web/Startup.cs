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
            //services.AddSingleton<AudioManager>();
            services.AddHostedService<ConnectionCoreHostedService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAllAddonsAndRequiredDiServices();
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


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, LoadedAddonHolder loadedAddonHolder, ILogger<Startup> logger, IServiceProvider serviceProvider)
        {
            //Todo - Should call initialize on all of the providers.
            foreach (var providerInterface in loadedAddonHolder.BapProviders)
            {
                var provider = (IBapProvider)serviceProvider.GetRequiredService(providerInterface.ProviderInterfaceType);
                ///ARRGGGG - find another place for this.
                await provider.InitializeAsync();
            }

            WebHostStartupMethods.SetupPages(loadedAddonHolder, logger);

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
