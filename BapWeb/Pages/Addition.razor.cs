using Microsoft.JSInterop;
using System.Net.NetworkInformation;
using System;
using BapShared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using BapWeb.Games;
using NLog;
using System.Threading.Tasks;
using BapButton;
using MessagePipe;
using MudBlazor;
using BapDb;
using System.Threading;

namespace BapWeb.Pages
{
    public partial class Addition : GamePage
    {

        [Inject]
        GameHandler GameHandler { get; set; } = default!;

        private AdditionGame game { get; set; } = default!;
        private bool TempUseTopRowAsDisplay { get; set; } = true;
        private List<int> MinAddend { get; set; } = new();
        private List<int> MaxAddend { get; set; } = new();
        private List<int> MaxValue { get; set; } = new();
        private List<int> TimeToPlay { get; set; } = new() { 30, 60, 90, 120, 150, 180, 210, 240, 270, 300 };
        private int _TempMinAddend { get; set; } = 0;
        private int _TempMaxAddend { get; set; } = 10;
        private int _TempMaxValue { get; set; } = 20;
        private int _TempTimeToPlay { get; set; } = 60;
        private bool TempIsSpanish { get; set; } = false;
        private int TempTimeToPlay
        {
            get
            {
                return _TempTimeToPlay;
            }
            set
            {
                _TempTimeToPlay = value;
            }
        }
        private int TempMinAddend
        {
            get
            {
                return _TempMinAddend;
            }
            set
            {
                _TempMinAddend = value;
                SettingUpdated();
            }
        }
        private int TempMaxAddend
        {
            get
            {
                return _TempMaxAddend;
            }
            set
            {
                _TempMaxAddend = value;
                SettingUpdated(true);
            }
        }
        private int TempMaxValue
        {
            get
            {
                return _TempMaxValue;
            }
            set
            {
                _TempMaxValue = value;
                SettingUpdated();
            }
        }

        private bool IsTopRowButtonDisplayPossible()
        {
            if (GameHandler.CurrentButtonLayout != null)
            {
                var topRow = GameHandler.CurrentButtonLayout.ButtonPositions.Where(t => t.RowId == 1).Count();
                if (topRow > 2)
                {
                    if (GameHandler.CurrentButtonLayout.ButtonPositions.Where(t => t.RowId != 1).Count() > 5)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private string GetSimpleDifficulty()
        {
            return game.GetDifficulty(TempMinAddend, TempMaxAddend).longVersion;
        }

        private void ShowHighScores(Score? newScore = null)
        {
            if (!game.IsGameRunning)
            {
                game.SetButtonCountAndButtonList(TempUseTopRowAsDisplay);
                var difficultyDetails = game.GetFullDifficultInfo(_TempMinAddend, _TempMaxAddend, game.ButtonCount);
                DialogOptions dialogOptions = new DialogOptions()
                {
                    CloseButton = false,
                    DisableBackdropClick = newScore != null
                };
                DialogParameters dialogParameters = new DialogParameters();
                dialogParameters.Add("NewScore", newScore);
                dialogParameters.Add("GameDataSaver", game?.DbSaver);
                dialogParameters.Add("Description", newScore?.DifficultyDetails ?? difficultyDetails.longVersion);
                dialogParameters.Add("Difficulty", newScore?.Difficulty ?? difficultyDetails.shortVersion);
                DialogService.Show<HighScoreTable>("High Scores", dialogParameters, dialogOptions);
            }

        }

        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
            game = (AdditionGame)GameHandler.UpdateToNewGameType(typeof(AdditionGame));
            if (!game.IsGameRunning)
            {
                game.Initialize();
            }
            SettingUpdated();
        }



        void SettingUpdated(bool maxAddendchanged = false)
        {
            if (TempMaxValue < TempMinAddend + TempMinAddend + 10)
            {
                _TempMaxValue = TempMinAddend + TempMinAddend + 10;
            }
            if (TempMaxValue > TempMaxAddend + TempMaxAddend)
            {
                _TempMaxValue = TempMaxAddend + TempMaxAddend;
            }
            if (TempMaxAddend < TempMinAddend)
            {
                _TempMaxAddend = TempMinAddend + 10;
            }
            if (maxAddendchanged)
            {
                _TempMaxValue = TempMaxAddend * 2;
            }
            MaxValue = new();
            for (int i = TempMinAddend + TempMaxAddend; i <= TempMaxAddend + TempMaxAddend; i = i + 5)
            {
                MaxValue.Add(i);
            }
            MinAddend = new();
            for (int i = 0; i <= 50; i = i + 5)
            {
                MinAddend.Add(i);
            }
            MaxAddend = new();
            for (int i = TempMinAddend + 5; i <= 90; i = i + 5)
            {
                MaxAddend.Add(i);
            }
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        public async override Task<bool> NodesChangedAsync(NodeChangeMessage _)
        {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
            return true;
        }

        public async override Task<bool> LayoutChangedAsync()
        {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
            return true;
        }


        public async override Task<bool> GameUpdateAsync(GameEventMessage e)
        {
            if (e.GameEnded)
            {
                StopUpdatingTime();

                Score highScore = game.GenerateScoreWithCurrentData();
                await InvokeAsync(() =>
                {
                    if (e.HighScoreAchieved)
                    {
                        ShowHighScores(highScore);
                    }

                    StateHasChanged();
                });
            }
            {
                await base.GameUpdateAsync(e);
            }
            return true;
        }
        async Task<bool> StartGame()
        {
            game.MinAddend = TempMinAddend;
            game.MaxAddend = TempMaxAddend;
            game.MaxValue = TempMaxValue;
            _ = KeepTimeUpdated();
            game.IsSpanish = TempIsSpanish;
            if (TempUseTopRowAsDisplay)
            {
                TempUseTopRowAsDisplay = IsTopRowButtonDisplayPossible();
            }
            await game.StartGame(TempTimeToPlay, TempUseTopRowAsDisplay);
            return true;
        }
        async Task<bool> EndGame()
        {
            await game.EndGame("Game Ended by User");
            return true;
        }


        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
