using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
    public interface IBapKeyboardProvider : IBapProvider, IDisposable
    {
        void ShowKeyboard(int turnOnInMillis = 0);
        void Enable();
        void Reset();
        void Disable(bool clearbuttons = true);
        void SetCharacters(string characters);
        void SetColor(BapColor color);
        void AddNodesToAvoid(List<string> nodeIds);
        void RemoveNodesToAvoid(List<string> nodeIds);
        void SetPlayDefaultSoundOnPress(bool playDefaultSoundOnPress);
        void OverrideButtonWithImage(char character, ButtonImage buttonImage, int timoutInMillis);
        bool IsEnabled { get; }
        BapColor DisplayColor { get; }
        List<string> NodesToAvoid { get; }
        List<string> ActiveNodes { get; }
        bool PlayDefaultSoundOnPress { get; }
    }

}
