using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BapShared
{
    public class AudioHandler
    {
        public List<(Type type, string Name)> TinkerSoundTypes { get; init; }
        private IServiceProvider Services { get; set; }
        public AudioHandler(IEnumerable<ITinkerSound> tinkerSounds, IServiceProvider services)
        {
            //I should probable inject the db context or some interface for fetching the default type.
            TinkerSounds = new List<ITinkerSound>();
            TinkerSoundTypes = tinkerSounds.Select(t => (t.GetType(), t.Name)).ToList();
            Services = services;
            foreach (var controller in tinkerSounds)
            {
                if (controller.GetType()?.FullName?.Contains("Web") == true)
                {
                    TinkerSounds.Add(controller);
                }
                else
                {
                    controller.Dispose();
                }
            }

        }

        public List<ITinkerSound> TinkerSounds { get; set; }
        public bool AddTinkerSound(Type type)
        {
            if (TinkerSounds.Where(t => t.GetType() == type).Count() == 0)
            {
                TinkerSounds.Add((ITinkerSound)Services.GetRequiredService(type));
                return true;
            }
            return false;
        }
        public bool RemoveTinkerSound(Type type)
        {
            ITinkerSound? soundToRemove = TinkerSounds.FirstOrDefault(t => t.GetType() == type);
            if (soundToRemove != null)
            {
                TinkerSounds.Remove(soundToRemove);
                return true;
            }
            return false;
        }
    }
}
