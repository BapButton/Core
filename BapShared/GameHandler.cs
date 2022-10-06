using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BapShared
{

    public class KeyboardHandler
    {
        public IKeyboardController KeyboardController { get; internal set; }
        private IServiceProvider services { get; set; }
        public ILogger<KeyboardHandler> Logger { get; internal set; }

        public KeyboardHandler(IServiceProvider serviceProvider, ILogger<KeyboardHandler> logger)
        {
            services = serviceProvider;
            Logger = logger;
            var keyboards = services.GetServices<IKeyboardController>();
            foreach (var keyboard in keyboards)
            {
                if (keyboard.GetType().Name.Contains("DefaultKeyboard"))
                {
                    KeyboardController = keyboard;
                }
            }
            if (KeyboardController == null)
            {
                KeyboardController = keyboards.FirstOrDefault()!;
            }
        }


        public T SetNewKeyboard<T>(bool reloadKeyboardIfSameTypeIsLoadded = false) where T : IKeyboardController
        {
            var keyboardType = typeof(T);
            if (KeyboardController != null && KeyboardController.GetType() == keyboardType && reloadKeyboardIfSameTypeIsLoadded == false)
            {
                return (T)KeyboardController;
            }

            if (KeyboardController != null)
            {
                KeyboardController.Dispose();
                KeyboardController = null;
            }

            KeyboardController = services.GetRequiredService<T>();
            return (T)KeyboardController;

        }
    }


    public class GameHandler
    {
        private IServiceProvider services { get; set; }
        public ITinkerGame? CurrentGame { get; internal set; }
        public ButtonLayout? CurrentButtonLayout { get; internal set; }

        public ILogger<GameHandler> Logger { get; internal set; }
        public IGameDescription? CurrentlySelectedGame { get; set; }
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
                return CurrentlySelectedGame != null;
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


        public ITinkerGame UpdateToNewGameType(Type gameType, bool createNewGameIfSameTypeLoaded = false)
        {
            if (CurrentGame != null && CurrentGame.GetType() == gameType && createNewGameIfSameTypeLoaded == false)
            {
                return CurrentGame;
            }
            if (gameType.GetInterfaces().Contains(typeof(ITinkerGame)))
            {
                if (CurrentGame != null)
                {
                    CurrentGame.Dispose();
                    CurrentGame = null;
                }

                CurrentGame = (ITinkerGame)services.GetRequiredService(gameType);
                return CurrentGame;
            }
            else
            {
                Logger.LogWarning($"{gameType.Name} does not implement interface ITinkerGame so it could not be started.");
            }
            return CurrentGame!;
        }
    }
}
