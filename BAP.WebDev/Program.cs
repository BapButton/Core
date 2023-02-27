using BAP.WebCore;
using BAP.WebDev;
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
builder.Services.AddHostedService<ConnectionCoreHostedService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAllAddonsAndRequiredDiServices(builder.Configuration);
builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.SetupPostDIBapServices();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapControllers();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

