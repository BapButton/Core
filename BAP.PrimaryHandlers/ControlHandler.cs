using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAP.Types;
using Microsoft.Extensions.DependencyInjection;

namespace BAP.PrimayHandlers
{
    public class ControlHandler : IControlHandler
    {
        public List<(Type type, string Name)> TinkerProviderTypes { get; init; }
        private IServiceProvider Services { get; set; }
        public ControlHandler(IEnumerable<IButtonProvider> allTinkerProviders, IServiceProvider services)
        {
            //I should probable inject the db context so I can get the default type.
            TinkerProviderTypes = allTinkerProviders.Select(t => (t.GetType(), t.Name)).ToList();
            Services = services;
            foreach (var Provider in allTinkerProviders)
            {
                if (Provider.GetType()?.FullName?.Contains("Mock") == false)
                {
                    CurrentButtonProvider = Provider;
                }
                else
                {
                    if (Provider != null)
                    {
                        Provider.Dispose();
                    }

                }
            }
            if (CurrentButtonProvider == null)
            {
                CurrentButtonProvider = (IButtonProvider)Services.GetRequiredService(TinkerProviderTypes.First().type);
            }
        }

        public IButtonProvider CurrentButtonProvider { get; set; }
        public bool ChangeButtonProvider(Type type)
        {
            IEnumerable<IButtonProvider> allProviders = Services.GetServices<IButtonProvider>();
            foreach (var Provider in allProviders)
            {
                if (Provider.GetType() == type)
                {
                    CurrentButtonProvider.Dispose();
                    CurrentButtonProvider = Provider;
                    CurrentButtonProvider.Initialize();
                }
                else
                {
                    Provider.Dispose();
                }
            }
            return true;
        }
    }
}
