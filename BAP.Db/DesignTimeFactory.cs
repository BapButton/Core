using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace BAP.Db
{
    public class ButtonContextFactory : IDesignTimeDbContextFactory<ButtonContext>
    {
        public ButtonContext CreateDbContext(string[] args)
        {

            IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddEnvironmentVariables()
            .Build();

            string connectionString = configuration.GetConnectionString("BAP.DBConnection") ?? "server=localhost;database=buttons;user=root;password=10640b03-1020-4be6-a065-634ea64c33c4";

            DbContextOptionsBuilder<ButtonContext> optionsBuilder = new DbContextOptionsBuilder<ButtonContext>();
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Using in memory DB");
                optionsBuilder.UseInMemoryDatabase(new Guid().ToString());
            }
            else
            {
                optionsBuilder.UseMySql(connectionString,
          MariaDbServerVersion.LatestSupportedServerVersion,
          mySqlOptions =>
              mySqlOptions.EnableRetryOnFailure(
                  maxRetryCount: 10,
                  maxRetryDelay: TimeSpan.FromSeconds(30),
                  errorNumbersToAdd: null));
            }


            return new ButtonContext(optionsBuilder.Options);
        }
    }
}
