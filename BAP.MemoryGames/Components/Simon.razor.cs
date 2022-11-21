using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MessagePipe;
using MudBlazor;
using BAP.Db;
using BAP.Types;
using BAP.Web;
using BAP.UIHelpers;
using BAP.UIHelpers.Components;

namespace BAP.MemoryGames.Components
{
    public partial class Simon : GamePage
    {
        [Inject]
        IGameHandler GameHandler { get; set; } = default!;
        [Inject]
        ILayoutHandler LayoutHandler { get; set; } = default!;
        [Inject]
        IBapMessageSender msgSender { get; set; } = default!;
        private SimonGame simonGame { get; set; } = default!;
        private List<int> ButtonCountList { get; set; } = new();
        private int ButtonsToUse { get; set; } = 0;
        private int RowToStartWith { get; set; } = 1;
        private List<int> ButtonRowList { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            base.OnInitialized();
            int buttonCount = msgSender.GetConnectedButtons().Count;
            GameHandler.UpdateToNewGameType(typeof(SimonGame));
            simonGame = (SimonGame)GameHandler.CurrentGame!;
            await ValidateButtonCount();
        }

        private void ShowHighScores(Score? newScore = null)
        {
            if (!simonGame.IsGameRunning)
            {
                //var difficultyDetails = simonGame.GetDifficulty(simonGame.RoundsCompleted);
                //DialogParameters dialogParameters = new DialogParameters
                //{
                //    { "NewScore", newScore },
                //    { "GameDataSaver", simonGame?.DbSaver },
                //    { "Description", newScore?.DifficultyDescription ?? difficultyDetails.longVersion },
                //    { "Difficulty", newScore?.DifficultyName ?? difficultyDetails.shortVersion }
                //};
                //DialogService.Show<HighScoreTable>("High Scores", dialogParameters);
            }

        }

        async Task<bool> StartGame()
        {
            simonGame.SetupGame(ButtonsToUse, RowToStartWith);
            await simonGame.Start();
            return true;
        }
        async Task<bool> EndGame(bool forceClosed = false)
        {
            await simonGame.EndGame("Game Ended by User", forceClosed);
            return true;
        }
        public async override Task<bool> GameUpdateAsync(GameEventMessage e)
        {

            await InvokeAsync(() =>
            {
                if (e.GameEnded && e.HighScoreAchieved)
                {
                    ShowHighScores(simonGame.GenerateScoreWithCurrentData());
                }
                StateHasChanged();
            });
            return true;
        }

        public async override Task<bool> NodesChangedAsync(NodeChangeMessage nodeChangeMessage)
        {
            return await ValidateButtonCount();

        }

        public async override Task<bool> LayoutChangedAsync()
        {
            return await ValidateButtonCount();
        }

        async Task<bool> ValidateButtonCount()
        {
            int maxButtons = Math.Min(msgSender.GetConnectedButtons().Count, simonGame.MaxButtonCount);
            if (maxButtons < 3)
            {
                if (simonGame.IsGameRunning)
                {
                    await EndGame(true);
                }
                return false;
            }
            ButtonCountList = Enumerable.Range(2, maxButtons - 1).ToList();
            if (ButtonsToUse > maxButtons)
            {
                ButtonsToUse = maxButtons;
            }
            if (ButtonsToUse < 2)
            {
                ButtonsToUse = maxButtons;
            }
            if (LayoutHandler.CurrentButtonLayout != null)
            {
                ButtonRowList = Enumerable.Range(1, LayoutHandler.CurrentButtonLayout.RowCount).ToList();
            }
            else
            {
                ButtonRowList = new() { 1 };
            }
            if (!ButtonRowList.Contains(RowToStartWith))
            {
                RowToStartWith = ButtonRowList.First();
            }
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
            return true;

        }
        public override void Dispose()
        {

        }
    }
}
