using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BAP.Types
{
    public interface IGameHandler
    {
        IBapGame? CurrentGame { get; }
        bool IsGameRunning { get; }
        bool IsGameSelected { get; }
        Task ForceGameEnd();
        IBapGame UpdateToNewGameType(Type gameType, bool createNewGameIfSameTypeLoaded = false);
    }
}