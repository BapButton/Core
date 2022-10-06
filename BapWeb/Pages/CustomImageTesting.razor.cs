using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BapWeb.Games;
using BapShared;

namespace BapWeb.Pages
{
    public partial class CustomImageTesting : ComponentBase, IDisposable
    {

        private CustomImageTest game = null;
        private string LastMessage = "";
        [Inject]
        private ILogger<CustomImageTesting> logger { get; set; } = default!;
        [Inject]
        private IServiceProvider services { get; set; } = default!;
        [Inject]
        private GameHandler gameHandler { get; set; } = default!;
        [Inject]
        ISubscriber<GameEventMessage> GameEventPipe { get; set; } = default!;
        IDisposable Subscriptions { get; set; } = default!;
        protected override void OnInitialized()
        {

            base.OnInitialized();
            if (gameHandler.CurrentGame != null)
            {
                game = (CustomImageTest)gameHandler.CurrentGame;
            }
            else
            {
                game = services.GetRequiredService<CustomImageTest>();
            }
            var bag = DisposableBag.CreateBuilder();
            GameEventPipe.Subscribe(async (x) => await GameUpdate(x)).AddTo(bag);
            Subscriptions = bag.Build();
            base.OnInitialized();

        }

        async Task GameUpdate(GameEventMessage e)
        {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        public async Task<bool> StartGame()
        {

            await game.Start();
            return true;
        }
        public async Task<bool> EndGame()
        {
            game.End("Game Ended by User");
            return true;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
