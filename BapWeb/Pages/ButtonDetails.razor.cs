using MessagePipe;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using BapButton;
using BapShared;




namespace BapWeb.Pages
{
    public partial class ButtonDetails : ComponentBase, IDisposable
    {
        [Inject]
        MessageSender msgSender { get; set; } = default!;
        [Inject]
        ISubscriber<NodeChangeMessage> nodeChanged { get; set; } = default!;
        IDisposable subscriptions = default!;




        protected override void OnInitialized()
        {
            var bag = DisposableBag.CreateBuilder();
            nodeChanged.Subscribe(async (x) => await ButtonCountChanged(x)).AddTo(bag);
            subscriptions = bag.Build();
            base.OnInitialized();

        }
        private async Task ButtonCountChanged(NodeChangeMessage e)
        {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private void SendBasicMessage(string nodeId)
        {
            StandardButtonCommand standardButtonCommand = new StandardButtonCommand();
            standardButtonCommand.InitialDisplay = new ButtonDisplay(new BapColor(0, 255, 0), Patterns.LetterI, turnOffAfterMillis: 1000);
            standardButtonCommand.OnPressDisplay = new ButtonDisplay(new BapColor(0, 255, 0), Patterns.LetterP, turnOffAfterMillis: 1000);
            standardButtonCommand.OnTimeOutDisplay = new ButtonDisplay(new BapColor(0, 255, 0), Patterns.LetterT);
            standardButtonCommand.TurnOnInMillis = 300;
            standardButtonCommand.WaitForCurrentTimerToComplete = false;
            msgSender.SendCommand(nodeId, standardButtonCommand);
        }
        private void RestartNode(string nodeId)
        {
            msgSender.RestartAButton(nodeId);
        }
        private void ShutdownNode(string nodeId)
        {
            msgSender.TurnOffAButton(nodeId);
        }
        private void IdentifyNode(string nodeId)
        {
            ButtonDisplay bd = new ButtonDisplay(0, 255, 0, Patterns.PlainSmilyFace, 0, 3000);
            msgSender.SendCommand(nodeId, new StandardButtonCommand(bd));
        }
        private void GetStatusOfNode(string nodeId)
        {
            msgSender.GetStatusFromAButtton(nodeId);
        }
        public void Dispose()
        {
            subscriptions?.Dispose();
        }
    }

}
