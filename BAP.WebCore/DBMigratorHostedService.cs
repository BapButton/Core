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
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace BAP.WebCore
{
    public class DBMigratorHostedService : IHostedService
    {
        ILogger<DBMigratorHostedService> _logger;
        IDbContextFactory<ButtonContext> _contextFactory;

        public DBMigratorHostedService(ILogger<DBMigratorHostedService> logger, IDbContextFactory<ButtonContext> contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Migrating the database");

            await DbMigrator.MigrateDatabaseIfNeeded(_contextFactory.CreateDbContext(), _logger);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

}
