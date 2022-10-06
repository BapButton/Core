using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using BapShared;
using BapWeb.Games;
using MessagePipe;
using Microsoft.Extensions.Logging;
using BapButton;

namespace BapWeb.Pages
{
    public partial class BasicGame : ComponentBase, IDisposable
    {
        private string LastMessage = "";
        private bool showLogs { get; set; } = false;
        [Inject]
        GameHandler GameHandler { get; set; } = default!;
        [Inject]
        ISubscriber<GameEventMessage> GameEventPipe { get; set; } = default!;
        IDisposable Subscriptions { get; set; } = default!;
        protected override void OnInitialized()
        {
            var bag = DisposableBag.CreateBuilder();
            GameEventPipe.Subscribe(async (x) => await GameUpdate(x)).AddTo(bag);
            Subscriptions = bag.Build();
            base.OnInitialized();
        }

        async Task GameUpdate(GameEventMessage e)
        {
            LastMessage = e.Message;
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }


        async Task<bool> ToggleLogs()
        {
            showLogs = !showLogs;
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
            return true;
        }

        async Task<bool> StartGame()
        {
            if (GameHandler.CurrentGame == null)
            {
                GameHandler.UpdateToNewGameType(typeof(TextMarqueGame));
            }
            if (GameHandler.CurrentGame != null)
            {
                await GameHandler.CurrentGame.Start();
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
                return true;
            }
            return false;
        }

        async Task<bool> EndGame()
        {
            if (GameHandler.CurrentGame != null)
            {
                await GameHandler.CurrentGame.ForceEndGame();
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
                return true;
            }
            return false;
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
