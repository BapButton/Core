using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BAP.Db
{
    public static class DbMigrator
    {
        public static async Task MigrateDatabaseIfNeeded(ButtonContext db, ILogger logger)
        {
            try
            {
                var allMigrations = db.Database.GetMigrations();
                var pendingMigrations = await db.Database.GetPendingMigrationsAsync();

                if (pendingMigrations.Any())
                {
                    Console.WriteLine($"You have {pendingMigrations.Count()} pending migrations to apply.");
                    Console.WriteLine("Applying pending migrations now");
                    await db.Database.MigrateAsync();
                }

                var lastAppliedMigration = (await db.Database.GetAppliedMigrationsAsync()).Last();

                Console.WriteLine($"You're on schema version: {lastAppliedMigration}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Migrating Database");
                Console.WriteLine($"error migrating the database {ex.Message} Inner exception {(ex?.InnerException?.Message ?? "")}");
            }
        }
    }
}
