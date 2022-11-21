using Microsoft.AspNetCore.Components;

namespace BAP.ReactionGames.Components
{
    public partial class SwordBonus : ComponentBase, IDisposable
    {
        [Inject]
        ILogger<SwordBonus> logger { get; set; } = default!;
        [Inject]
        IBapMessageSender MsgSender { get; set; } = default!;
        [Inject]
        IGameHandler GameHandler { get; set; } = default!;
        [Inject]
        ISubscriber<GameEventMessage> GameEventPipe { get; set; } = default!;
        private string updateMessage = "";
        private string message = "";
        private SwordBonusGame game { get; set; } = default!;

        private Timer _timer;
        private ReactionGameStatus status;

        IDisposable Subscriptions { get; set; } = default!;
        protected override void OnInitialized()
        {

            base.OnInitialized();
            int buttonCount = MsgSender.GetConnectedButtons().Count;
            message = $"There {(buttonCount > 1 ? "are" : "is")} {buttonCount} Button{(buttonCount > 1 ? "s" : "")} connected";
            var bag = DisposableBag.CreateBuilder();
            GameEventPipe.Subscribe(async (x) => await GameUpdate(x)).AddTo(bag);
            Subscriptions = bag.Build();
            status = game.GetStatus();
            GameHandler.UpdateToNewGameType(typeof(SwordBonusGame));
            game = (SwordBonusGame)GameHandler.CurrentGame!;
        }

        void StartTime()
        {
            _timer = new Timer(async _ =>
            {
                await InvokeAsync(StateHasChanged);
            }, null, 0, 1000);

        }

        async Task<bool> StartGame()
        {
            status = game.GetStatus();
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
        async Task GameUpdate(GameEventMessage e)
        {
            updateMessage = e.Message;
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
