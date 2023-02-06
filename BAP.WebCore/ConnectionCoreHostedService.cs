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
using Microsoft.EntityFrameworkCore.Internal;

namespace BAP.WebCore
{
    public class ConnectionCoreHostedService : IHostedService
    {
        ILogger<ConnectionCoreHostedService> _logger;
        IDbContextFactory<ButtonContext> _contextFactory;

        public ConnectionCoreHostedService(ILogger<ConnectionCoreHostedService> logger, IDbContextFactory<ButtonContext> contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Migrating the database");
            using ButtonContext db = _contextFactory.CreateDbContext();
            try
            {
                await db.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Migrating Database");
                Console.WriteLine($"error migrating the database {ex.Message} Inner exception {(ex?.InnerException?.Message ?? "")}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

}
