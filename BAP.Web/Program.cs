using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAP.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).ConfigureAppConfiguration((builderContext, config) =>
            {
                config.AddEnvironmentVariables();
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            }).Build().Run();
            //var weatherService = host.Services.GetRequiredService<WeatherService>();
            //await weatherService.InitializeWeatherAsync(
            //    host.Configuration["WeatherServiceUrl"]);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    //.UseKestrel()
                    .UseStartup<Startup>();
                    //.UseUrls("http://0.0.0.0:5000");
                }).ConfigureLogging(logging =>
                {
                    //logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddNLogWeb();
                });

        //.UseNLog();
    }
}
