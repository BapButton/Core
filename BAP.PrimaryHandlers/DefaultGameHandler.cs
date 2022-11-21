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
using Microsoft.AspNetCore.Components;
using BAP.Types;
using BAP.PrimaryHandlers;

namespace BAP.PrimayHandlers
{
    public class DefaultGameHandler : IGameHandler
    {
        ILoadablePageHandler _loadablePageHandler { get; init; }

        private IServiceProvider _services { get; set; }
        public IBapGame? CurrentGame { get; internal set; }

        public ILogger<DefaultGameHandler> Logger { get; internal set; }

        public DefaultGameHandler(IServiceProvider serviceProvider, ILogger<DefaultGameHandler> logger, ILoadablePageHandler loadablePageHandler)
        {
            _services = serviceProvider;
            Logger = logger;
            _loadablePageHandler = loadablePageHandler;
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
                return _loadablePageHandler.SelectedItemIsAGame;
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

                CurrentGame = (IBapGame)_services.GetRequiredService(gameType);
                return CurrentGame;
            }
            else
            {
                Logger.LogWarning($"{gameType.Name} does not implement interface IBapGame so it could not be started.");
            }
            return CurrentGame!;
        }
    }
}
