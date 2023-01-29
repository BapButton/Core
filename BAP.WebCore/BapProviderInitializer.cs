using BAP.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.WebCore
{
    internal class BapProviderInitializer : IHostedService
    {
        // We need to inject the IServiceProvider so we can create 
        // the scoped service, MyDbContext
        private readonly IServiceProvider _serviceProvider;
        private DbAccessor _dba;
        public BapProviderInitializer(IServiceProvider serviceProvider, DbAccessor dbAccessor)
        {
            _serviceProvider = serviceProvider;
            _dba = dbAccessor;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            LoadedAddonHolder addonHolder = _serviceProvider.GetRequiredService<LoadedAddonHolder>();
            foreach (var providerInterface in addonHolder.BapProviders)
            {
                var provider = (IBapProvider)_serviceProvider.GetRequiredService(providerInterface.ProviderInterfaceType);
                await provider.InitializeAsync();
                if (providerInterface?.ProviderInterfaceType?.FullName != null)
                {
                    if (_dba.GetRecentlyActiveProvider(providerInterface.ProviderInterfaceType.FullName).Count == 0)
                    {
                        _dba.AddActiveProvider(provider.GetType(), false);
                    }
                }

            }


        }

        // noop
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
