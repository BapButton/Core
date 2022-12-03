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
    [GamePage("Ethan Sword Game", "Ethans version of the Sword game with a special bonus round.", "0fe4493b-714d-465d-8425-f49b3bd91289")]
    public partial class ReactionEthan : GamePage
    {
        [Inject]
        IBapMessageSender MsgSender { get; set; } = default!;
        [Inject]
        IGameHandler GameHandler { get; set; } = default!;

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

