using BAP.Db;
using FluentValidation;
using MessagePipe;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BAP.Web.Models;
using BAP.Web.TTS;
using Scrutor;
using BAP.Web.Games;
using MockButtonCore;
using MudBlazor.Services;
using BAP.Types;

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
			services.AddSingleton<GameHandler>();
			services.AddSingleton<ControlHandler>();
			services.AddHostedService<ConnectionCoreHostedService>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddHttpClient<ITtsService, TtsService>(client => client.BaseAddress = new Uri("http://localhost:5002/api/"));
			services.AddTransient<IValidator<FileUpload>, FileUploadValidator>();
			services.AddTransient<MockConnectionCore>();
			services.AddDbContextFactory<ButtonContext>();
			services.AddTransient(p => p.GetRequiredService<IDbContextFactory<ButtonContext>>().CreateDbContext());
			services.AddTransient<DbAccessor>();
			services.AddSingleton<KeyboardHandler>();
            services.AddSingleton<AnimationController>();
			services.AddMessagePipe();
			services.Scan(scan => scan.FromApplicationDependencies()
					.AddClasses(t => { t.AssignableTo<IGameDataSaver>(); })
					  .AsImplementedInterfaces()
					  .WithTransientLifetime());
			services.Scan(scan => scan.FromApplicationDependencies()
					.AddClasses(t => { t.AssignableTo<IBapMessageSender>(); })
					  .AsImplementedInterfaces()
					  .WithTransientLifetime());
			services.Scan(scan => scan.FromApplicationDependencies()
					.AddClasses(t => { t.AssignableTo<IBapButtonProvider>(); })
					  .AsImplementedInterfaces()
					  .WithTransientLifetime());
			services.Scan(scan => scan.FromCallingAssembly()
					.AddClasses(t => { t.AssignableTo<IBapGame>(); })
					  .AsSelf()
					  .WithTransientLifetime());
			services.Scan(scan => scan.FromCallingAssembly()
					.AddClasses(t => { t.AssignableTo<IBapGameDescription>(); })
					  .AsImplementedInterfaces().WithSingletonLifetime());
			services.Scan(scan => scan.FromCallingAssembly()
					.AddClasses(t => { t.AssignableTo<IBapGameDescription>(); })
					  .AsSelf().WithSingletonLifetime());
			services.Scan(scan => scan.FromApplicationDependencies()
					.AddClasses(t => { t.AssignableTo<IBapKeyboardProvider>(); })
						.AsSelf().WithTransientLifetime());

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

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ButtonContext db)
		{

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
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
