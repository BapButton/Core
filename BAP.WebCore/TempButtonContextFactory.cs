using BAP.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.WebCore
{
    internal class TempButtonContextFactory : IDbContextFactory<ButtonContext>
    {
        private DbContextOptions<ButtonContext> _options;
        public TempButtonContextFactory(string connectionString)
        {
            DbContextOptionsBuilder<ButtonContext> options = new DbContextOptionsBuilder<ButtonContext>();
            if (string.IsNullOrEmpty(connectionString))
            {

                options.UseInMemoryDatabase(new Guid().ToString());
                _options = options.Options;
            }
            else
            {
                options.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion);
                _options = options.Options;
            }
        }
        public ButtonContext CreateDbContext()
        {
            return new ButtonContext(_options);
        }
    }
}
