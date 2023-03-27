using BAP.TestUtilities;
using BAP.WebCore;
using BAP.WebDev;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NLog;
using NLog.Web;
using SixLabors.ImageSharp;
using System.Reflection;

var logger = LogManager.Setup().GetCurrentClassLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMvc();
builder.Services.AddControllers();
builder.AddAllAddonsAndRequiredDiServices();



var app = builder.Build();

app.UseExceptionHandler("/Error");

app.SetupPostDIBapServices();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapControllers();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

