using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MockButtonCore;
using BapButton;
using BapShared;
using Microsoft.Extensions.DependencyInjection;
using BapDb;


namespace BapWeb.Pages
{
    public partial class ButtonTesting : ComponentBase, IDisposable
    {
        [Parameter]
        public bool HideButtonsWhenGameActive { get; set; } = false;
        private string lastButtonMessage = "";
        [Inject]
        ControlHandler CtrlHandler { get; set; } = default!;
        [Inject]
        GameHandler GameHandler { get; set; } = default!;
        //[Inject]
        //ILogger<MockConnectionCore> Logger { get; set; } = default!;
        //[Inject]
        //IServiceProvider Services { get; set; } = default!;
        IDisposable subscriptions = default!;

        [Inject]
        ISubscriber<GameStateChangedMessage> gameStateChangedPipe { get; set; } = default!;
        [Inject]
        ISubscriber<ButtonPressedMessage> buttonPressedPipe { get; set; } = default!;
        [Inject]
        ISubscriber<NodeChangeMessage> nodeChangePipe { get; set; } = default!;

        [Inject]
        ISubscriber<LayoutChangeMessage> layoutChangePipe { get; set; } = default!;

        MockConnectionCore Core { get; set; } = default!;


        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (CtrlHandler.CurrentController == null || CtrlHandler.CurrentController.GetType() != typeof(MockConnectionCore))
            {
                CtrlHandler.ChangeTinkerController(typeof(MockConnectionCore));
            }
            Core = (MockConnectionCore)CtrlHandler.CurrentController!;
            var bag = DisposableBag.CreateBuilder();
            gameStateChangedPipe.Subscribe(async (x) => await Updates(x)).AddTo(bag);
            buttonPressedPipe.Subscribe(async (x) => await ButtonPressed(x)).AddTo(bag);
            nodeChangePipe.Subscribe(async (x) => await ButtonChanged()).AddTo(bag);
            layoutChangePipe.Subscribe(async (x) => await ButtonChanged()).AddTo(bag);
            subscriptions = bag.Build();

        }

        async Task Updates(GameStateChangedMessage e)
        {
            await InvokeAsync(() =>
             {
                 StateHasChanged();
             });
        }
        async Task ButtonPressed(ButtonPressedMessage e)
        {
            lastButtonMessage = $"{e.NodeId} - {e.ButtonPress.MillisSinceLight}";
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        async Task ButtonChanged()
        {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        protected void AddButton()
        {
            Core.AddNode();
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
