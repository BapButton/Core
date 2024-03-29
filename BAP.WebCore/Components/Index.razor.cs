﻿using BAP.Db;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BAP.WebCore.Components
{ public partial class Index : ComponentBase, IDisposable
    {
        [Inject]
        ISubscriber<GameEventMessage> GameEventPipe { get; set; } = default!;
        [Inject]
        DbAccessor dba { get; set; } = default!;
        [Inject]
        IGameProvider GameHandler { get; set; } = default!;
        [Inject]
        ILayoutProvider LayoutProvider { get; set; } = default!;
        [Inject]
        ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        IKeyboardProvider KeyboardProvider { get; set; } = default!;
        [Inject]
        IBapMessageSender msgSender { get; set; } = default!;
        [Inject]
        ISubscriber<NodeChangeMessage> NodeChangedPipe { get; set; } = default!;
        [Inject]
        ISubscriber<LayoutChangeMessage> LayoutChangePipe { get; set; } = default!;
        DynamicComponent? dc { get; set; }
        List<(string uniqueId, int playCount)> GamePlayStatistics { get; set; } = new List<(string uniqueId, int playCount)>();
        IDisposable Subscriptions { get; set; } = default!;
        bool IsGameSelected
        {
            get
            {
                return GameHandler.IsGameSelected;
            }
        }
        [Inject]
        LoadedAddonHolder AddonHolder { get; set; } = default!;

        List<GameDetail> AllGames
        {
            get
            {
                List<GameDetail> AllGames = AddonHolder.AllGames.ToList();
                return AllGames;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            var bag = DisposableBag.CreateBuilder();
            GameEventPipe.Subscribe(async (x) => await Updates(x)).AddTo(bag);
            NodeChangedPipe.Subscribe(async (x) => await NodeChanged(x)).AddTo(bag);
            LayoutChangePipe.Subscribe(async (x) => await LayoutChanged()).AddTo(bag);
            Subscriptions = bag.Build();
            GamePlayStatistics = await dba.GetGamePlayStatistics();
        }

        List<GameDetail> GetOrderedGameDescriptions()
        {

            //Need to add some favorites functionality in here as well. 
            foreach (var game in AllGames)
            {
                if (GamePlayStatistics.Where(t => t.uniqueId == game.UniqueId).Count() == 0)
                {
                    GamePlayStatistics.Add((game.UniqueId, 0));
                }
            }
            var list = (from g in AllGames
                        join p in GamePlayStatistics on g.UniqueId equals p.uniqueId
                        orderby p.playCount descending
                        select g).ToList();
            return list;
        }
        async Task Updates(GameEventMessage message)
        {
            if (message.FatalError)
            {
                Snackbar.Add(message.Message, Severity.Error);
            }
            if (dc != null && dc.Instance != null)
            {
                if (dc.Instance is IGamePage gamePage)
                {
                    await gamePage.GameUpdateAsync(message);
                }
            }
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });

        }
        async Task NodeChanged(NodeChangeMessage nodeChangeMessage)
        {
            if (dc != null && dc.Instance != null)
            {
                if (dc.Instance is IGamePage gamePage)
                {
                    await gamePage.NodesChangedAsync(nodeChangeMessage);
                }
            }
        }

        async Task LayoutChanged()
        {
            if (dc != null && dc.Instance != null)
            {
                if (dc.Instance is IGamePage gamePage)
                {
                    await gamePage.LayoutChangedAsync();
                }
            }
        }

        private void DeselectGame()
        {
            //todo - Need to unload the game page so it goes back to default;
            GameHandler?.ForceGameEnd();
            GameHandler?.DeselectGame();
            msgSender.ClearAllCachedAudio();
            if (KeyboardProvider.IsEnabled)
            {
                KeyboardProvider.Disable();
            }

            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private async Task SelectGame(GameDetail gameDetail)
        {
            await dba.AddGamePlayLog(gameDetail.UniqueId);
            GameHandler.UpdateDynamicComponentToLoad(gameDetail.DynamicComponentToLoad, gameDetail.Name, gameDetail.Description, gameDetail.UniqueId);
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        public void Dispose()
        {
            Subscriptions.Dispose();
        }
    }
}
