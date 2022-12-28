using BAP.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BAP.Types;

namespace BAP.Web
{
    public class ConnectionCoreHostedService : IHostedService
    {
        IButtonProvider ButtonProvider { get; set; }

        public ConnectionCoreHostedService(ILogger<ConnectionCoreHostedService> logger, IButtonProvider buttonProvider)
        {
            ButtonProvider = buttonProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Migrating the database");
            using ButtonContext db = new();
            try
            {
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error migrating the database {ex.Message} Inner exception {(ex?.InnerException?.Message ?? "")}");
            }

            if (ButtonProvider != null)
            {
                bool succesfullyInitialized = await ButtonProvider.InitializeAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

}
