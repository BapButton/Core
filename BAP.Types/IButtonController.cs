using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAP.Types
{
    public interface IButtonProvider : IBapProvider, IDisposable
    {
        List<string> GetConnectedButtons();
        List<string> GetConnectedButtonsInOrder();
        List<(string nodeId, ButtonStatus buttonStatus)> GetAllConnectedButtonInfo();
    }
}