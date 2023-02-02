using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using MessagePipe;
using BAP.Types;
using BAP.Helpers;

namespace BAP.ReactionGames.Components
{

    public partial class Reaction : ComponentBase, IDisposable
    {
        [Inject]
        IGameProvider GameHandler { get; init; } = default!;
        [Inject]
        IBapMessageSender MsgSender { get; init; } = default!;
        [Inject]
        ISubscriber<GameEventMessage> GameEventPipe { get; set; } = default!;
        IDisposable Subscriptions { get; set; } = default!;
        private string updateMessage = "";
        private string message = "";
        private ReactionGame game { get; set; } = default!;

        protected override void OnInitialized()
        {

            base.OnInitialized();
            int buttonCount = MsgSender.GetConnectedButtons().Count;
            message = $"There {(buttonCount > 1 ? "are" : "is")} {buttonCount} Button{(buttonCount > 1 ? "s" : "")} connected";
            var bag = DisposableBag.CreateBuilder();
            GameEventPipe.Subscribe(async (x) => await Updates(x)).AddTo(bag);
            Subscriptions = bag.Build();

            game = (ReactionGame)GameHandler.UpdateToNewGameType(typeof(ReactionGame));
        }
        async Task Updates(GameEventMessage e)
        {
            updateMessage = e.Message;
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        async Task<bool> StartGame()
        {
            await game.Start(60);
            return true;
        }
        async Task<bool> EndGame()
        {
            await game.EndGame("Game Ended by User");
            return true;
        }

        public void Dispose()
        {
            if (Subscriptions != null)
            {
                Subscriptions.Dispose();
            }
        }
    }
}
