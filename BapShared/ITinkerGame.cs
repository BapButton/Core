using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BapShared
{
    public interface ITinkerGame : IDisposable
    {
        public bool IsGameRunning { get; }
        public Task<bool> Start();
        public Task<bool> ForceEndGame();
    }
}

