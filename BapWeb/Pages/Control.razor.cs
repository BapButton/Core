using MessagePipe;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BapButton;
using BapShared;

namespace BapWeb.Pages
{

    public partial class Control : ComponentBase, IDisposable
    {

        [Inject]
        ISubscriber<NodeChangeMessage> nodeChanged { get; set; } = default!;
        [Inject]
        ControlHandler CtrlHandler { get; set; } = default!;
        [Inject]
        MessageSender MsgSender { get; set; } = default!;
        [Inject]
        IEnumerable<ITinkerController> AllControllers { get; set; } = default!;
        IDisposable subscriptions = default!;
        string mosquittoAddress = "";
        bool isConnected = false;
        bool isStarted = false;
        string verb = "is";
        protected override void OnInitialized()
        {
            var bag = DisposableBag.CreateBuilder();
            nodeChanged.Subscribe(async (x) => await ButtonCountChanged(x)).AddTo(bag);
            subscriptions = bag.Build();
            base.OnInitialized();
            mosquittoAddress = Environment.GetEnvironmentVariable("MosquittoAddress") ?? "";
           
            
            if (CtrlHandler.CurrentController.GetType() == typeof(TinkerConnectionCore))
            {
                var client = ((TinkerConnectionCore)CtrlHandler.CurrentController).managedMqttClient;
                isConnected = client?.IsConnected ?? false;
                isStarted = client?.IsStarted ?? false;
            }
        }
        
        private void CheckButtonStatus()
        {
            MsgSender.GetStatusFromAllButttons();
        }
        private void RestartButtons()
        {
            MsgSender.RestartAllButtons();
        }
        private void TurnOffButtons()
        {
            MsgSender.TurnOffAllButtons();
        }

        private void SelectController(Type controllerType)
        {
            CtrlHandler.ChangeTinkerController(controllerType);
        }
        private async Task ButtonCountChanged(NodeChangeMessage e)
        {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        public void Dispose()
        {
            subscriptions?.Dispose();
        }
    }
}
