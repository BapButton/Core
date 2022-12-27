using BAP.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.WebCore
{
    public class DefaultProviderChanger : IBapProviderChanger
    {
        LoadedAddonHolder _loadedAddonHolder { get; init; }

        Dictionary<Type, IBapProvider> CurrentlyActiveBapProviders { get; } = new();
        Dictionary<Type, List<IBapProvider>> CurrentlyActiveMultipleBapProviders { get; } = new();
        IServiceProvider serviceProvider { get; init; }
        DefaultProviderChanger(LoadedAddonHolder loadedAddonHolder)
        {
            _loadedAddonHolder = loadedAddonHolder;
        }

        public bool SetNewBapProvider<T>(Type typeOfNewItem) where T : IBapProvider
        {
            if (CurrentlyActiveBapProviders.TryGetValue(typeof(T), out var provider))
            {
                provider.Dispose();
            }
            CurrentlyActiveBapProviders[typeof(T)] = (IBapProvider)serviceProvider.GetRequiredService(typeOfNewItem);
            return true;
        }

        public bool AddBapProvider<T>(Type typeOfNewItem) where T : IBapProvider
        {
            throw new NotImplementedException();
        }


        public bool RemoveBapProvider<T>(Type typeOfNewItem) where T : IBapProvider
        {
            throw new NotImplementedException();
        }

        public T? GetCurrentBapProvider<T>() where T : IBapProvider
        {
            if (CurrentlyActiveBapProviders.TryGetValue(typeof(T), out var provider))
            {
                return (T)provider;
            }
            return default(T);
        }


        public List<T> GetCurrentBapProviders<T>() where T : IBapProvider
        {
            //T must implement Allow Multiple
            List<T> correctproviders = new();
            if (CurrentlyActiveMultipleBapProviders.TryGetValue(typeof(T), out var providers))
            {
                foreach (var provider in providers)
                {
                    if (provider is T)
                    {
                        providers.Add(provider);
                    }
                }
                return correctproviders;
            }
            return correctproviders;
        }

        public List<(Type loadableType, string uniqueId, string name, string description)> GetAvailableBapProviders<T>() where T : IBapProvider
        {
            List<(Type loadableType, string uniqueId, string name, string description)> providers = new();
            foreach (var provider in _loadedAddonHolder.BapProviders.Where(t => typeof(T).IsAssignableFrom(t.BapProviderType)))
            {
                providers.Add((provider.BapProviderType, provider.UniqueId, provider.Name, provider.Description));
            }
            return providers;
        }
    }
}
