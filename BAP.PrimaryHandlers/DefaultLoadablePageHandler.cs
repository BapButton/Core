using BAP.PrimayHandlers;
using BAP.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.PrimaryHandlers
{
    public class DefaultLoadablePageHandler : ILoadablePageHandler
    {
        private IServiceProvider _services { get; set; }
        public IMainAreaItem? CurrentlySelectedItem { get; set; }

        public ILogger<DefaultLoadablePageHandler> Logger { get; internal set; }

        public DefaultLoadablePageHandler(IServiceProvider serviceProvider, ILogger<DefaultLoadablePageHandler> logger)
        {
            _services = serviceProvider;
            Logger = logger;
        }

        public bool UpdateCurrentlySelectedItem(IMainAreaItem mainAreaItem, bool createNewGameIfSameTypeLoaded = false)
        {
            if (CurrentlySelectedItem != null && CurrentlySelectedItem.GetType() == mainAreaItem.GetType() && createNewGameIfSameTypeLoaded == false)
            {
                return true;
            }
            CurrentlySelectedItem = mainAreaItem;
            if (mainAreaItem.TypeOfInitialDisplayComponent.GetInterfaces().Contains(typeof(IComponent)))
            {
                CurrentlySelectedItem = mainAreaItem;
                return true;
            }
            else
            {
                Logger.LogWarning($"{mainAreaItem.TypeOfInitialDisplayComponent.Name} does not implement interface IComponent so it could not be started.");
                return false;
            }
            return true;
        }

        public bool SelectedItemIsAGame
        {
            get
            {
                return CurrentlySelectedItem?.GetType()?.GetInterfaces().Contains(typeof(IBapGameDescription)) ?? false;
            }
        }
    }
}
