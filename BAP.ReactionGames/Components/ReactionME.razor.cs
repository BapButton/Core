using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Timers;
using BAP.Types;
using BAP.Helpers;
using BAP.Web;

namespace BAP.ReactionGames.Components
{
    [GamePage("Mads and Ethan Reaction Game", "Mads and Ethan's version of a reaction game", "bdf32580-a2e7-4b30-9e4b-42aa51f58a2c")]
    public partial class ReactionME : GamePage
    {
        [Inject]
        IBapMessageSender MsgSender { get; set; } = default!;
        [Inject]
        IGameProvider GameHandler { get; set; } = default!;
        //[Inject]
        //ISubscriber<GameEventMessage> GameEventPipe { get; set; } = default!;
        //IDisposable Subscriptions { get; set; } = default!;

        private string updateMessage = "";
        private string message = "";
        private ReactionGameME game = null;
        private System.Threading.Timer _timer;
        private ReactionGameStatus status = new ReactionGameStatus();

        protected override void OnInitialized()
        {

            base.OnInitialized();
            game = (ReactionGameME)GameHandler.UpdateToNewGameType(typeof(ReactionGameME));
            //var bag = DisposableBag.CreateBuilder();
            //GameEventPipe.Subscribe(async (x, _) => await GameUpdates(x)).AddTo(bag);
            //Subscriptions = bag.Build();
        }
        protected override void OnAfterRender(bool firstRender)
        {
            status = game.GetStatus();
            message = $"There {(MsgSender.GetConnectedButtons().Count > 1 ? "are" : "is")} {MsgSender.GetConnectedButtons().Count} Button{(MsgSender.GetConnectedButtons().Count > 1 ? "s" : "")} connected";
            base.OnAfterRender(firstRender);
        }

        void StartTime()
        {
            _timer = new System.Threading.Timer(async _ =>
            {
                await InvokeAsync(StateHasChanged);
            }, null, 0, 1000);

        }

        async Task<bool> StartGame()
        {
            MsgSender.StopAllAudio();
            await game.Start(60);
            StartTime();
            return true;
        }
        async Task<bool> EndGame()
        {
            await game.EndGame("Game Ended by User");
            return true;
        }
        //async Task GameUpdates(GameEventMessage e)
        //{
        //    updateMessage = e.Message;
        //    await InvokeAsync(() =>
        //    {
        //        StateHasChanged();
        //    });
        //}


        public override void Dispose()
        {
            //if (Subscriptions != null)
            //{
            //    Subscriptions.Dispose();
            //}
        }
    }
}

