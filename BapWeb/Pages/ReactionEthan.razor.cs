using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Timers;
using BapWeb.Games;
using BapButton;
using BapShared;

namespace BapWeb.Pages
{
    public partial class ReactionEthan : GamePage
    {
        [Inject]
        MessageSender MsgSender { get; set; } = default!;
        [Inject]
        GameHandler GameHandler { get; set; } = default!;

        private string updateMessage = "";
        private string message = "";
        private ReactionGameEthan game { get; set; } = default!;
        private System.Threading.Timer _timer;
        private ReactionGameStatus status = new ReactionGameStatus();

        protected override void OnInitialized()
        {

            base.OnInitialized();
            game = (ReactionGameEthan)GameHandler.UpdateToNewGameType(typeof(ReactionGameEthan));
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
            if (!game.IsGameRunning)
            {
                game = (ReactionGameEthan)GameHandler.UpdateToNewGameType(typeof(ReactionGameEthan), true);
            }
            await game.Start(60);
            StartTime();
            return true;
        }
        async Task<bool> EndGame()
        {
            game.ForceEndBonusGame();
            await game.EndGame("Game Ended by User");
            return true;
        }


        public override void Dispose()
        {

        }
    }
}

