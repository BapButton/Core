using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BAP.TestUtilities.Components
{

    [MenuItem("Button Testing", "Test out with fake buttons", false, "ea0d8577-609f-40e4-b05e-f02004b080a4")]
    public partial class ButtonTesting : ComponentBase, IDisposable
    {
        [Parameter]
        public bool HideButtonsWhenGameActive { get; set; } = false;
        private string lastButtonMessage = "";
        [Inject]
        IButtonProvider ButtonProvider { get; set; } = default!;
        [Inject]
        IGameProvider GameHandler { get; set; } = default!;
        [Inject]
        ILayoutProvider LayoutProvider { get; set; } = default!;
        //[Inject]
        //ILogger<MockConnectionCore> Logger { get; set; } = default!;
        //[Inject]
        //IServiceProvider Services { get; set; } = default!;
        IDisposable subscriptions = default!;

        [Inject]
        ISubscriber<GameEventMessage> gameStateChangedPipe { get; set; } = default!;
        [Inject]
        ISubscriber<ButtonPressedMessage> buttonPressedPipe { get; set; } = default!;
        [Inject]
        ISubscriber<NodeChangeMessage> nodeChangePipe { get; set; } = default!;

        [Inject]
        ISubscriber<LayoutChangeMessage> layoutChangePipe { get; set; } = default!;

        MockButtonProvider Core { get; set; } = default!;


        protected override void OnInitialized()
        {
            base.OnInitialized();
            Core = (MockButtonProvider)ButtonProvider!;
            var bag = DisposableBag.CreateBuilder();
            gameStateChangedPipe.Subscribe(async (x) => await Updates(x)).AddTo(bag);
            buttonPressedPipe.Subscribe(async (x) => await ButtonPressed(x)).AddTo(bag);
            nodeChangePipe.Subscribe(async (x) => await ButtonChanged()).AddTo(bag);
            layoutChangePipe.Subscribe(async (x) => await ButtonChanged()).AddTo(bag);
            subscriptions = bag.Build();

        }

        async Task Updates(GameEventMessage e)
        {
            await InvokeAsync(() =>
             {
                 StateHasChanged();
             });
        }
        async Task ButtonPressed(ButtonPressedMessage e)
        {
            lastButtonMessage = $"{e.NodeId}";
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

        protected async Task AddButton()
        {
            await Core.AddNode();
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
