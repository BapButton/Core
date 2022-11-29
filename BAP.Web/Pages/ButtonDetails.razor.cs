using MessagePipe;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BAP.Web.Pages
{
    [MenuItem(DisplayedLabel = "Button Details", MouseOverText = "Info about connected buttons")]
    public partial class ButtonDetails : ComponentBase, IDisposable
    {
        [Inject]
        IBapMessageSender msgSender { get; set; } = default!;
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
            BapColor bc = new BapColor(0, 255, 0);
            msgSender.SendImage(nodeId, new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.LetterI), bc));
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
            BapColor bc = new BapColor(0, 255, 0);
            msgSender.SendImage(nodeId, new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.PlainSmilyFace), bc));
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
