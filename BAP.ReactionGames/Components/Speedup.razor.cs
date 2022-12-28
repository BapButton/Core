using BAP.UIHelpers.Components;
using Microsoft.AspNetCore.Components;

namespace BAP.ReactionGames.Components
{
    [GamePage("Speed Up", "Game slowly speeds up. How long can you keep the board from filling up without making too many mistakes?", "27f39a0a-914f-4563-bbb0-b8511474956f")]
    public partial class SpeedUp : GamePage
    {
        [Inject]
        IGameHandler GameHandler { get; set; } = default!;
        private ReactionSpeedup game { get; set; } = default!;

        internal TimeSpan TimePlayed
        {
            get
            {
                return game.GameLength;
            }
        }
        private void ShowHighScores(Score? newScore = null)
        {
            if (!game.IsGameRunning)
            {
                int buttonCount = game.buttons.Count;
                if (buttonCount == 0)
                {
                    buttonCount = game.MsgSender.ButtonCount;
                }
                var (shortVersion, longVersion) = game.GetFullDifficulty(buttonCount);
                DialogOptions dialogOptions = new()
                {
                    CloseButton = false,
                    DisableBackdropClick = newScore != null
                };
                DialogParameters dialogParameters = new()
                {
                    { "NewScore", newScore },
                    { "GameDataSaver", game?.DbSaver },
                    { "Description", newScore?.DifficultyDescription ?? longVersion },
                    { "Difficulty", newScore?.DifficultyDescription ?? shortVersion }
                };
                DialogService.Show<HighScoreTable>("High Scores", dialogParameters, dialogOptions);
            }

        }



        public async override Task<bool> GameUpdateAsync(GameEventMessage e)
        {
            if (e.GameEnded)
            {
                StopUpdatingTime();

                await InvokeAsync(() =>
                {
                    if (e.HighScoreAchieved)
                    {
                        Score highScore = game.GenerateScoreWithCurrentData();
                        ShowHighScores(highScore);
                    }

                    StateHasChanged();
                });
            }
            else
            {
                await base.GameUpdateAsync(e);
            }
            return true;
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            game = (ReactionSpeedup)GameHandler.UpdateToNewGameType(typeof(ReactionSpeedup));
            if (!game.IsGameRunning)
            {
                game.Initialize();
            }
        }
        public async void EndGame()
        {
            StopUpdatingTime();
            await game.EndSpeedupGame("Closed by player", true);
        }

        public async void StartGame()
        {
            if (!game.IsGameRunning)
            {
                _ = KeepTimeUpdated();
                await game.Start();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
