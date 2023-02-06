using BAP.UIHelpers;
using BAP.WebCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NLog.Targets;
using SixLabors.ImageSharp;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMvc();
builder.Services.AddControllers();
builder.Services.AddHostedService<ConnectionCoreHostedService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAllAddonsAndRequiredDiServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
MethodCallTarget target = new MethodCallTarget("LiveLogger", (logEvent, parms) => LiveLogger.RecordNewLogMessage(logEvent.LoggerName, logEvent.Level, logEvent.FormattedMessage));
NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, NLog.LogLevel.Trace);
app.Services.GetRequiredService<LoadedAddonHolder>();
WebHostStartupMethods.SetupPages(app.Services.GetRequiredService<LoadedAddonHolder>(), app.Logger);

app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapControllers();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
