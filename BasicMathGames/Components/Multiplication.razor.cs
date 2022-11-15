using Microsoft.JSInterop;
using System.Net.NetworkInformation;
using System;
using BapShared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Threading.Tasks;
using MessagePipe;
using MudBlazor;
using BAP.Db;
using System.Threading;
using BAP.Types;
using BAP.Helpers;
using BAP.UIHelpers;

namespace BAP.BasicMathGames
{
    public partial class Multiplication : GamePage
    {

        [Inject]
        GameHandler GameHandler { get; set; } = default!;

        private MultiplicationGame game { get; set; } = default!;
        private bool TempUseTopRowAsDisplay { get; set; } = true;
        private List<int> MinNumber { get; set; } = new();
        private List<int> MaxNumber { get; set; } = new();
        private List<int> MaxValue { get; set; } = new();
        private List<int> TimeToPlay { get; set; } = new() { 30, 60, 90, 120, 150, 180, 210, 240, 270, 300 };
        private int _TempMinNumber { get; set; } = 0;
        private int _TempMaxNumber { get; set; } = 10;
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
        private int TempMinNumber
        {
            get
            {
                return _TempMinNumber;
            }
            set
            {
                _TempMinNumber = value;
                SettingUpdated();
            }
        }
        private int TempMaxNumber
        {
            get
            {
                return _TempMaxNumber;
            }
            set
            {
                _TempMaxNumber = value;
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
            return game.GetDifficulty(TempMinNumber, TempMaxNumber).longVersion;
        }

        private void ShowHighScores(Score? newScore = null)
        {
            if (!game.IsGameRunning)
            {
                game.SetButtonCountAndButtonList(TempUseTopRowAsDisplay);
                var difficultyDetails = game.GetFullDifficultInfo(_TempMinNumber, _TempMaxNumber, game.ButtonCount);
                DialogOptions dialogOptions = new DialogOptions()
                {
                    CloseButton = false,
                    DisableBackdropClick = newScore != null
                };
                DialogParameters dialogParameters = new DialogParameters();
                dialogParameters.Add("NewScore", newScore);
                dialogParameters.Add("GameDataSaver", game?.DbSaver);
                dialogParameters.Add("Description", newScore?.DifficultyDescription ?? difficultyDetails.longVersion);
                dialogParameters.Add("Difficulty", newScore?.DifficultyName ?? difficultyDetails.shortVersion);
                DialogService.Show<HighScoreTable>("High Scores", dialogParameters, dialogOptions);
            }

        }

        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
            game = (MultiplicationGame)GameHandler.UpdateToNewGameType(typeof(MultiplicationGame));
            if (!game.IsGameRunning)
            {
                game.Initialize();
            }
            SettingUpdated();
        }



        void SettingUpdated(bool maxNumberchanged = false)
        {
            if (TempMaxValue < TempMinNumber * TempMinNumber + 10)
            {
                _TempMaxValue = TempMinNumber * TempMinNumber + 10;
            }
            if (TempMaxValue > TempMaxNumber * TempMaxNumber)
            {
                _TempMaxValue = TempMaxNumber * TempMaxNumber;
            }
            if (TempMaxNumber < TempMinNumber)
            {
                _TempMaxNumber = TempMinNumber + 5;
            }
            if (maxNumberchanged)
            {
                _TempMaxValue = TempMaxNumber * TempMinNumber;
            }
            MaxValue = new();
            for (int i = TempMinNumber * TempMaxNumber; i <= TempMaxNumber * TempMaxNumber; i = i + 5)
            {
                MaxValue.Add(i);
            }
            MinNumber = new();
            for (int i = 0; i <= 50; i = i + 5)
            {
                MinNumber.Add(i);
            }
            MaxNumber = new();
            for (int i = TempMinNumber + 5; i <= 90; i = i + 5)
            {
                MaxNumber.Add(i);
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
            game.Multiplicand = TempMinNumber;
            game.Multiplier = TempMaxNumber;
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
