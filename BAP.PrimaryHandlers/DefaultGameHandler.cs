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
    [BapProvider("Default Gamer Provider", "Holds games in Memory and can close and enable them", "96c3fe0c-8051-4777-986f-daf88ac1afe1")]
    public class DefaultGameProvider : IGameProvider
    {
        private IServiceProvider _services { get; set; }
        public IBapGame? CurrentGame { get; internal set; }
        public string CurrentGameFullName { get; internal set; } = "";

        public ILogger<DefaultGameProvider> Logger { get; internal set; }

        public DefaultGameProvider(IServiceProvider serviceProvider, ILogger<DefaultGameProvider> logger)
        {
            _services = serviceProvider;
            Logger = logger;
        }

        public bool IsGameRunning
        {
            get
            {
                return CurrentGame?.IsGameRunning ?? false;
            }
        }



        public string CurrentGameName { get; private set; } = "";

        public string CurrentGameUniqueId { get; private set; } = "";

        public string CurrentGameDescription { get; private set; } = "";

        public Type? DynamicComponentToLoad { get; set; } = null;

        public bool IsGameSelected => CurrentGame != null;

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

                CurrentGame = (IBapGame)ActivatorUtilities.CreateInstance(_services, gameType);
                CurrentGameFullName = gameType?.FullName ?? "";
                return CurrentGame;
            }
            else
            {
                Logger.LogWarning($"{gameType.Name} does not implement interface IBapGame so it could not be started.");
            }
            return CurrentGame!;
        }

        public bool UpdateDynamicComponentToLoad(Type pageToLoad, string gameName, string gameDescription, string gameUniqueId)
        {
            //Need to check that the type is valid;
            CurrentGameName = gameName;
            CurrentGameDescription = gameDescription;
            CurrentGameUniqueId = gameUniqueId;
            DynamicComponentToLoad = pageToLoad;
            return true;
        }

        public Task DeselectGame()
        {
            if (CurrentGame != null)
            {
                CurrentGame.Dispose();
                DynamicComponentToLoad = null;
                CurrentGameDescription = "";
                CurrentGameUniqueId = "";
                CurrentGameName = "";
                CurrentGame = null;
            }
            return Task.CompletedTask;
        }

        public async Task<bool> InitializeAsync()
        {
            return await Task.FromResult(true);
        }

        public void Dispose()
        {

        }
    }
}
