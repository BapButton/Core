using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
    public interface IBapGame : IDisposable
    {
        public bool IsGameRunning { get; }
        public Task<bool> Start();
        public Task<bool> ForceEndGame();
    }
}

