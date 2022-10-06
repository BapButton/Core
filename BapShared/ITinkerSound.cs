using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BapShared
{
    public interface ITinkerSound :IDisposable
    {
        string Name { get; }
        Task<bool> Initialize();
        Task<(bool success, string message)> PlaySound(string pathToAudioFile);
    }
}
