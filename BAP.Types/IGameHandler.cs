using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BAP.Types
{
    public interface IGameHandler
    {
        IBapGame? CurrentGame { get; }
        string GameFullName
        {
            get
            {
                return CurrentGame?.GetType().FullName ?? "No Game Loaded"
;
            }
        }
        bool IsGameRunning { get; }
        bool IsGameSelected { get; }
        Task ForceGameEnd();
        IBapGame UpdateToNewGameType(Type gameType, bool createNewGameIfSameTypeLoaded = false);
    }
}