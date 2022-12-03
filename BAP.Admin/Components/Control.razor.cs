using MessagePipe;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BAP.Types;
using BAP.Helpers;

namespace BAP.Admin.Components
{
    [MenuItem("Control", "Controls and info about all buttons",true, "45944587-8f34-417a-9911-93d9a9297edd")]
    public partial class Control : ComponentBase, IDisposable
    {

        [Inject]
        ISubscriber<NodeChangeMessage> nodeChanged { get; set; } = default!;
        [Inject]
        IControlHandler CtrlHandler { get; set; } = default!;
        [Inject]
        IBapMessageSender MsgSender { get; set; } = default!;
        [Inject]
        IEnumerable<IButtonProvider> AllControllers { get; set; } = default!;
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


            if (CtrlHandler.CurrentButtonProvider.GetType() == typeof(MqttBapButtonProvider))
            {
                var client = ((MqttBapButtonProvider)CtrlHandler.CurrentButtonProvider).managedMqttClient;
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
            CtrlHandler.ChangeButtonProvider(controllerType);
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
