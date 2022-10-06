using MQTTnet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BapShared
{
    public interface ITinkerController : IDisposable
    {
        string ControllerName { get; }
        List<string> GetConnectedButtons();
        List<string> GetConnectedButtonsInOrder();
        List<(string nodeId, ButtonStatus buttonStatus)> GetAllConnectedButtonInfo();
        Task<bool> Initialize(string mqttServerIp = "");
    }



}