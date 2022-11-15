using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{



	public class GameHandler
	{
		private IServiceProvider services { get; set; }
		public IBapGame? CurrentGame { get; internal set; }
		public ButtonLayout? CurrentButtonLayout { get; internal set; }
		public List<IBapGameDescription> Games { get; internal set; }

		public ILogger<GameHandler> Logger { get; internal set; }
		public IBapGameDescription? CurrentlySelectedGame { get; set; }
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
			else
			{
				Logger.LogWarning($"{gameType.Name} does not implement interface ITinkerGame so it could not be started.");
			}
			return CurrentGame!;
		}
	}
}
