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
		public List<(IBapAudioProvider providers, string Name)> ActiveAudioProviders { get; init; }
		private IServiceProvider Services { get; set; }
		public AudioManager(DbAccessor dbAccessor, IEnumerable<IBapAudioProvider> audioProviders, IServiceProvider services)
		{
			BapSounds = new List<IBapAudioProvider>();
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

		public List<IBapAudioProvider> BapSounds { get; set; }
		public bool AddBapSound(Type type)
		{
			if (BapSounds.Where(t => t.GetType() == type).Count() == 0)
			{
				BapSounds.Add((IBapAudioProvider)ActivatorUtilities.CreateInstance(Services, type));
				return true;
			}
			return false;
		}
		public bool RemoveBapSound(Type type)
		{
			IBapAudioProvider? soundToRemove = BapSounds.FirstOrDefault(t => t.GetType() == type);
			if (soundToRemove != null)
			{
				BapSounds.Remove(soundToRemove);
				return true;
			}
			return false;
		}
	}
}
