using MessagePack.Resolvers;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using MockButtonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BapButton;
using BapShared;

namespace BapWeb.Pages
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
        MessageSender MsgSender { get; set; } = default!;
        [Inject]
        ControlHandler CtrlHandler { get; set; } = default!;
        [Inject]
        GameHandler GameHandler { get; set; } = default!;
        [Inject]
        ISubscriber<CustomImageMessage> customImageRcieved { get; set; } = default!;
        [Inject]
        ISubscriber<InternalCustomImageMessage> internalCustomImageRecieved { get; set; } = default!;
        [Inject]
        ISubscriber<StandardButtonCommandMessage> standardButtonMessage { get; set; } = default!;
        private InternalCustomImage? InternalCustomImage { get; set; } = null;
        private StandardButtonCommand CurrentMessage { get; set; } = new();
        private ButtonDisplay CurrentDisplayedItem { get; set; } = new ButtonDisplay();
        private DateTime LightTurnedOnTime { get; set; }
        private DateTime LightTurnedOffTime { get; set; }
        private System.Timers.Timer turnOffTimer { get; set; } = default!;
        private System.Timers.Timer turnOnTimer { get; set; } = default!;
        private Dictionary<int, ulong[]> CustomImages { get; set; } = default!;
        private bool OriginalLightDisplayIsStillShowing { get; set; } = true;
        private NextButtonDisplay NextButtonDisplay { get; set; } = new();

        protected override void OnInitialized()
        {
            CustomImages = new Dictionary<int, ulong[]>();
            var bag = DisposableBag.CreateBuilder(); // composite disposable for manage subscription

            standardButtonMessage.Subscribe(async (x) => await StandardButtonMessage(x)).AddTo(bag);
            customImageRcieved.Subscribe(async (x) => await CustomImageMessage(x)).AddTo(bag);
            internalCustomImageRecieved.Subscribe(async (x) => await InternalCustomImageMessage(x)).AddTo(bag);

            subscriptions = bag.Build();
            turnOffTimer = new System.Timers.Timer();
            turnOffTimer.Elapsed += TurnOffLightEvent;
            turnOnTimer = new System.Timers.Timer();
            turnOnTimer.Elapsed += TurnOnLightEvent;
        }
        private void TurnOffLightEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            turnOffTimer.Stop();
            ChangeDisplay();
        }
        private void TurnOnLightEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            turnOffTimer.Stop();
            ChangeDisplay();
        }
        async Task CustomImageMessage(CustomImageMessage e)
        {
            if (e.NodeId == "" || e.NodeId == NodeId)
            {
                ulong[,] data = new ulong[8, 8];
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        data[r, c] = e.CustomImage.ImageData[r * 8 + c];
                    }
                }
                InternalCustomImage = new InternalCustomImage(data);
            }
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        async Task InternalCustomImageMessage(InternalCustomImageMessage e)
        {
            if (e.NodeId == "" || e.NodeId == NodeId)
            {
                InternalCustomImage = e.CustomImage;
            }
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }


        async Task StandardButtonMessage(StandardButtonCommandMessage e)
        {
            if (e.NodeId == "" || e.NodeId == NodeId)
            {
                InternalCustomImage = null;
                CurrentMessage = e.StandardButtonMessage;
                if (CurrentMessage.TurnOnInMillis > 0)
                {
                    turnOnTimer.Interval = CurrentMessage.TurnOnInMillis;
                    turnOnTimer.Start();
                }
                else
                {
                    NextButtonDisplay = NextButtonDisplay.Initial;
                    ChangeDisplay();
                }

            }

        }

        protected void ChangeDisplay()
        {

            switch (NextButtonDisplay)
            {
                case NextButtonDisplay.Blank:
                    //no code is need here. It already got set to blank.
                    break;
                case NextButtonDisplay.Initial:
                    CurrentDisplayedItem = CurrentMessage.InitialDisplay;
                    NextButtonDisplay = NextButtonDisplay.Timeout;
                    break;
                case NextButtonDisplay.Timeout:
                    CurrentDisplayedItem = CurrentMessage.OnTimeOutDisplay;
                    if (CurrentDisplayedItem.TurnOffAfterMillis > 0)
                    {
                        NextButtonDisplay = NextButtonDisplay.Blank;
                    }
                    break;
                case NextButtonDisplay.OnPress:
                    CurrentDisplayedItem = CurrentMessage.OnPressDisplay;
                    NextButtonDisplay = NextButtonDisplay.Timeout;
                    break;
                default:
                    break;
            }
            if (NextButtonDisplay == NextButtonDisplay.Timeout || NextButtonDisplay == NextButtonDisplay.OnPress)
            {
                LightTurnedOffTime = DateTime.Now;
            }
            if (CurrentDisplayedItem.TurnOffAfterMillis > 0)
            {
                turnOffTimer.Stop();
                turnOffTimer.Interval = CurrentDisplayedItem.TurnOffAfterMillis;
                turnOffTimer.Start();
            }
            InvokeAsync(() =>
            {
                StateHasChanged();
            });

        }


        protected async Task<bool> ClickButton()
        {
            ulong timeSinceLightTurnedOff = 0;
            if (NextButtonDisplay == NextButtonDisplay.Blank)
            {
                timeSinceLightTurnedOff = (ulong)(DateTime.Now - LightTurnedOffTime).TotalMilliseconds;
            }
            ButtonPress bp = new()
            {
                MillisSinceLight = (ulong)(DateTime.Now - LightTurnedOnTime).TotalMilliseconds,
                TimeSinceLightTurnedOff = timeSinceLightTurnedOff
            };
            NextButtonDisplay = NextButtonDisplay.OnPress;
            ChangeDisplay();
            MsgSender.MockClickButton(NodeId, bp);

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
            return true;
        }
        protected void RemoveButton()
        {
            if (CtrlHandler.CurrentController.GetType() == typeof(MockConnectionCore))
            {
                ((MockConnectionCore)CtrlHandler.CurrentController).RemoveNode(NodeId);
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
