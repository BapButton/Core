using Microsoft.AspNetCore.Components;
using BAP.Web.Games;
using System.Linq;
using Blazored.FluentValidation;
using BAP.Db;
using BAP.UIHelpers.Components;
//using SpotifyAPI.Web;

namespace BAP.TextGames.Components
{
    [GamePage("Vocab Game", "Working on spelling your Vocab words", UniqueId = "4512ba27-1f61-4b72-a795-6ce512f940e7")]
    public partial class Vocab : GamePage
    {
        [Inject]
        IGameProvider GameHandler { get; set; } = default!;
        [Inject]
        ILayoutProvider LayoutProvider { get; set; } = default!;
        //[Inject]
        //NavigationManager NavManager { get; set; } = default!;

        private VocabGame game { get; set; } = default!;

        public bool UseTestingButtons
        {
            get
            {
                return game.useTestingButtons;
            }
            set
            {
                game.useTestingButtons = value;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            game = (VocabGame)GameHandler.UpdateToNewGameType(typeof(VocabGame));
            if (!game.IsGameRunning)
            {
                game.Initialize();
            }
        }

        //private void AuthorizeSpotify()
        //{
        //    var loginRequest = new LoginRequest(
        //          new Uri("https://localhost:5001/spotifyCallback/"),
        //          "4a9b9dab320240659583880719c5816d",
        //          LoginRequest.ResponseType.Code
        //        )
        //    {
        //        Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.UserReadCurrentlyPlaying, Scopes.UserReadPlaybackPosition, Scopes.UserReadPlaybackState, Scopes.UserModifyPlaybackState }
        //    };
        //    var uri = loginRequest.ToUri();
        //    NavManager.NavigateTo(uri.ToString(), true);
        //}

        private void ShowHighScores(Score? newScore = null)
        {
            if (!game.IsGameRunning)
            {
                game.SetButtonCountAndButtonList(false);
                DialogOptions dialogOptions = new DialogOptions()
                {
                    CloseButton = false,
                    DisableBackdropClick = newScore != null
                };
                DialogParameters dialogParameters = new DialogParameters();
                dialogParameters.Add("NewScore", newScore);
                dialogParameters.Add("GameDataSaver", game?.DbSaver);
                dialogParameters.Add("Description", newScore?.DifficultyDescription ?? "");
                dialogParameters.Add("Difficulty", newScore?.DifficultyId ?? "");
                DialogService.Show<HighScoreTable>("High Scores", dialogParameters, dialogOptions);
            }
        }

        async Task<bool> StartGame()
        {
            _ = KeepTimeUpdated();

            await game.Start();
            return true;
        }
        async Task<bool> EndGame()
        {
            await game.EndGame("Game Ended by User");
            return true;
        }

        private void ShowVocabGameSetup()
        {
            if (!game.IsGameRunning)
            {
                DialogOptions dialogOptions = new DialogOptions()
                {
                    CloseButton = false,
                    DisableBackdropClick = true
                };
                DialogParameters dialogParameters = new DialogParameters();
                dialogParameters.Add("GameDataSaver", game?.DbSaver);
                DialogService.Show<VocabGameSetup>("Update Vocab Words", dialogParameters, dialogOptions);
            }

        }


        public override Task<bool> GameUpdateAsync(GameEventMessage gameEventMessage)
        {
            if (gameEventMessage.Message.StartsWith("VocabGameUpdated"))
            {
                game.RefreshSavedVocab();
            }
            return base.GameUpdateAsync(gameEventMessage);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

    }
}
