﻿using Microsoft.AspNetCore.Components;

namespace BAP.TestUtilities
{
    public enum NextButtonDisplay
    {
        Blank = 0,
        Initial = 1,
        Timeout = 2,
        OnPress = 3
    }
    public partial class MockButton : ComponentBase, IDisposable
    {
        [Parameter]
        public bool HideButtonsWhenGameActive { get; set; } = false;
        [Parameter]
        public string NodeId { get; set; } = "";
        IDisposable subscriptions = default!;
        [Inject]
        ILogger<MockButton> Logger { get; set; } = default!;
        [Inject]
        IBapMessageSender MsgSender { get; set; } = default!;
        [Inject]
        IButtonProvider ButtonProvider { get; set; } = default!;
        [Inject]
        IGameProvider GameHandler { get; set; } = default!;
        [Inject]
        ISubscriber<StandardButtonImageMessage> standardButtonMessage { get; set; } = default!;
        private StandardButtonImageMessage CurrentMessage { get; set; } = default!;
        private ButtonImage CurrentDisplayedItem { get; set; } = new ButtonImage();
        private DateTime LightTurnedOnTime { get; set; }
        private DateTime LightTurnedOffTime { get; set; }

        protected override void OnInitialized()
        {
            var bag = DisposableBag.CreateBuilder(); // composite disposable for manage subscription

            standardButtonMessage.Subscribe(async (x) => await StandardButtonMessage(x)).AddTo(bag);

            subscriptions = bag.Build();
        }

        async Task StandardButtonMessage(StandardButtonImageMessage e)
        {
            if (e.NodeId == "" || e.NodeId == NodeId)
            {
                CurrentMessage = e;
                CurrentDisplayedItem = e.ButtonImage;
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }

        }


        protected async Task<bool> ClickButton()
        {

            ButtonPress bp = new();
            MsgSender.MockClickButton(NodeId, bp);
            return true;
        }
        protected async Task RemoveButton()
        {
            var currentButtonProvider = ButtonProvider;
            if (currentButtonProvider.GetType() == typeof(MockButtonProvider))
            {
                await ((MockButtonProvider)currentButtonProvider).RemoveNode(NodeId);
            }
        }

        public void Dispose()
        {
            if (subscriptions != null)
            {
                subscriptions.Dispose();
            }

        }
    }
}
