using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

//TODO = This is the problem with having gamehandler down here which needs to have web like logic. 
//using Microsoft.AspNetCore.Components;

namespace BAP.Types
{



    public class GameHandler
    {
        private IServiceProvider services { get; set; }
        public bool SelectedItemIsAGame
        {
            get
            {
                return CurrentlySelectedItem?.GetType()?.GetInterfaces().Contains(typeof(IBapGameDescription)) ?? false;
            }
        }
        public IBapGame? CurrentGame { get; internal set; }
        public ButtonLayout? CurrentButtonLayout { get; internal set; }
        public ILogger<GameHandler> Logger { get; internal set; }
        public IMainAreaItem? CurrentlySelectedItem { get; set; }
        public GameHandler(IServiceProvider serviceProvider, ILogger<GameHandler> logger)
        {
            services = serviceProvider;
            Logger = logger;
        }

        public bool IsGameRunning
        {
            get
            {
                return CurrentGame?.IsGameRunning ?? false;
            }
        }
        public bool IsGameSelected
        {
            get
            {
                return CurrentlySelectedItem != null;
            }
        }
        public async Task ForceGameEnd()
        {
            if (CurrentGame != null)
            {
                try
                {
                    await CurrentGame.ForceEndGame();
                }
                catch (Exception ex)
                {
                    Logger.LogWarning($"Failed to end Game with error {ex.Message}");
                }
                CurrentGame?.Dispose();
                CurrentGame = null;
            }

        }

        public ButtonLayout? SetNewButtonLayout(ButtonLayout? bl)
        {
            CurrentButtonLayout = bl;
            return bl;
        }

        public bool UpdateCurrentlySelectedItem(IMainAreaItem mainAreaItem, bool createNewGameIfSameTypeLoaded = false)
        {
            if (CurrentlySelectedItem != null && CurrentlySelectedItem.GetType() == mainAreaItem.GetType() && createNewGameIfSameTypeLoaded == false)
            {
                return true;
            }
            CurrentlySelectedItem = mainAreaItem;
            //if (mainAreaItem.TypeOfInitialDisplayComponent.GetInterfaces().Contains(typeof(IComponent)))
            //{
            //    CurrentlySelectedItem = mainAreaItem;
            //    return true;
            //}
            //else
            //{
            //    Logger.LogWarning($"{mainAreaItem.TypeOfInitialDisplayComponent.Name} does not implement interface IComponent so it could not be started.");
            //    return false;
            //}
            return true;
        }

        public IBapGame UpdateToNewGameType(Type gameType, bool createNewGameIfSameTypeLoaded = false)
        {
            if (CurrentGame != null && CurrentGame.GetType() == gameType && createNewGameIfSameTypeLoaded == false)
            {
                return CurrentGame;
            }
            if (gameType.GetInterfaces().Contains(typeof(IBapGame)))
            {
                if (CurrentGame != null)
                {
                    CurrentGame.Dispose();
                    CurrentGame = null;
                }

                CurrentGame = (IBapGame)ActivatorUtilities.CreateInstance(services, gameType);
                return CurrentGame;
            }
            else if (gameType.GetInterfaces().Contains(typeof(IMainAreaItem)))
            {
                CurrentlySelectedItem = (IMainAreaItem)ActivatorUtilities.CreateInstance(services, gameType);
            }
            else
            {
                Logger.LogWarning($"{gameType.Name} does not implement interface IBapGame so it could not be started.");
            }
            return CurrentGame!;
        }
    }
}
