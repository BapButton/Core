using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BapShared
{
    public interface IKeyboardController : IDisposable
    {
        bool IsEnabled { get; }
        bool IsConfigured { get; }
        void ShowKeyboard(int turnOnInMillis = 0);
        void Enable();
        void Disable(bool clearbuttons = false);
    }
}
