using BAP.Db;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace BAP.Web.Pages
{
    public partial class Index : ComponentBase, IDisposable
    {
        [Inject]
        ISubscriber<GameEventMessage> GameEventPipe { get; set; } = default!;
        [Inject]
        DbAccessor dba { get; set; } = default!;
        [Inject]
        IGameHandler GameHandler { get; set; } = default!;
        [Inject]
        ILayoutHandler LayoutHandler { get; set; } = default!;
        [Inject]
        ILoadablePageHandler LoadablePageHandler { get; set; } = default!;
        [Inject]
        ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        IKeyboardHandler KeyboardHandler { get; set; } = default!;
        [Inject]
        IBapMessageSender msgSender { get; set; } = default!;
        [Inject]
        ISubscriber<NodeChangeMessage> NodeChangedPipe { get; set; } = default!;
        [Inject]
        ISubscriber<LayoutChangeMessage> LayoutChangePipe { get; set; } = default!;
        int countOfGames { get; set; }
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
        //This needs to go away. I need to write a find method and get rid of the DI stuff - it is just easy. 
        [Inject]
        IEnumerable<GameDetail> DiGames { get; set; } = default!;

        [Inject]
        LoadedAddonHolder AddonHolder { get; set; } = default!;

        List<GameDetail> AllGames
        {
            get
            {
                List<GameDetail> AllGames = AddonHolder.AllGames.ToList();
                AllGames.AddRange(DiGames);
                return AllGames;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            countOfGames = AllGames.Count();
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

            LoadablePageHandler.CurrentlySelectedItem = null;
            GameHandler?.ForceGameEnd();
            msgSender.ClearAllCachedAudio();
            if (KeyboardHandler.CurrentKeyboard.IsEnabled)
            {
                KeyboardHandler.CurrentKeyboard.Disable();
            }

            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private async Task SelectGame(GameDetail gameDetail)
        {
            await dba.AddGamePlayLog(gameDetail.UniqueId);
            LoadablePageHandler.CurrentlySelectedItem = gameDetail;
        }
        public void Dispose()
        {
            Subscriptions.Dispose();
        }
    }
}
