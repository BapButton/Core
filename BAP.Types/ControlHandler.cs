using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BAP.Types
{
	public class ControlHandler
	{
		public List<(Type type, string Name)> TinkerProviderTypes { get; init; }
		private IServiceProvider Services { get; set; }
		public ControlHandler(IEnumerable<IBapButtonProvider> allTinkerProviders, IServiceProvider services)
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
				CurrentButtonProvider = (IBapButtonProvider)Services.GetRequiredService(TinkerProviderTypes.First().type);
			}
		}

		public IBapButtonProvider CurrentButtonProvider { get; set; }
		public bool ChangeButtonProvider(Type type)
		{
			IEnumerable<IBapButtonProvider> allProviders = Services.GetServices<IBapButtonProvider>();
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
