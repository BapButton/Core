using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAP.Types;
using BAP.Db;

namespace BAP.Helpers
{
	public class AudioManager
	{
		public List<(IAudioProvider providers, string Name)> ActiveAudioProviders { get; init; }
		private IServiceProvider Services { get; set; }
		public AudioManager(DbAccessor dbAccessor, IEnumerable<IAudioProvider> audioProviders, IServiceProvider services)
		{
			BapSounds = new List<IAudioProvider>();
			//Todo - This needs to be fixed. The type is wrong and using result is wrong.
			List<string> previousProviders = dbAccessor.GetRecentlyActiveProvider<IBapProvider>().Result;
			Services = services;
			foreach (var controller in audioProviders)
			{
				if (controller.GetType()?.FullName?.Contains("Web") == true)
				{
					BapSounds.Add(controller);
				}
				else
				{
					controller.Dispose();
				}
			}

		}

		public List<IAudioProvider> BapSounds { get; set; }
		public bool AddBapSound(Type type)
		{
			if (BapSounds.Where(t => t.GetType() == type).Count() == 0)
			{
				BapSounds.Add((IAudioProvider)ActivatorUtilities.CreateInstance(Services, type));
				return true;
			}
			return false;
		}
		public bool RemoveBapSound(Type type)
		{
			IAudioProvider? soundToRemove = BapSounds.FirstOrDefault(t => t.GetType() == type);
			if (soundToRemove != null)
			{
				BapSounds.Remove(soundToRemove);
				return true;
			}
			return false;
		}
	}
}
