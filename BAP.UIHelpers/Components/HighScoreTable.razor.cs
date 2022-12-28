using BAP.Types;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAP.UIHelpers.Components;
public partial class HighScoreTable : ComponentBase, IDisposable
{

    [Parameter]
    public Score? NewScore { get; set; } = null;
    [Parameter]
    public string Difficulty { get; set; } = default!;

    [Parameter]
    public IGameDataSaver GameDataSaver { get; set; } = default!;

    [Parameter]
    public string Description { get; set; } = default!;
    public List<Score> Scores { get; set; } = new();
    [Parameter]
    public bool? HigherScoreIsBetter { get; set; } = true;
    [Inject]
    public IKeyboardProvider KeyboardProvider { get; set; } = default!;
    [Inject]
    ISubscriber<KeyboardKeyPressedMessage> KeyboardKeyPipe { get; set; } = default!;
    [Inject]
    private IDialogService DialogService { get; set; } = default!;
    public string NewUserName { get; set; } = "";
    IDisposable Subscriptions { get; set; } = default!;
    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; } = default!;
    List<(string difficulty, string difficultyDetails)> CurrentScoreBoards { get; set; } = new();
    int CurrentLocationInScoreBoards { get; set; }


    protected override async Task OnInitializedAsync()
    {

        var bag = DisposableBag.CreateBuilder();
        KeyboardKeyPipe.Subscribe(async (x) => await UpdateUsername(x.KeyValue)).AddTo(bag);
        Subscriptions = bag.Build();

        if (NewScore == null)
        {
            Scores = await GameDataSaver.GetScores(Difficulty, 10, HigherScoreIsBetter ?? true);
        }
        else
        {
            Scores = await GameDataSaver.GetScoresWithNewScoreIfWarranted(NewScore, 10, HigherScoreIsBetter ?? true);
            if (Scores.Where(t => t.ScoreId == 0).Any())
            {
                KeyboardProvider.SetCharacters(BasicKeyboardLetters.EnglishUpperCaseLetters);
                KeyboardProvider.Reset();
                KeyboardProvider.ShowKeyboard();
            }
        }
        CurrentScoreBoards = await GameDataSaver.GetCurrentScoreBoards();
        //CurrentLocationInScoreBoards = CurrentScoreBoards.IndexOf()

    }


    public async Task CloseHighScores()
    {
        if (Scores.Where(t => t.ScoreId == 0).Any())
        {
            bool? result = await DialogService.ShowMessageBox(
            "Warning",
            "Exit without saving your high score?",
            yesText: "Throw away score", cancelText: "Go back to saving");
            if (result.HasValue && result == true)
            {
                MudDialog.Close();
            }
            StateHasChanged();
        }
        else
        {
            MudDialog.Close();
        }
    }

    public async Task UpdateUsername(char newChar)
    {
        NewUserName += newChar;
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }

    public async Task SaveNewHighScore()
    {
        if (NewUserName.Length > 2)
        {
            Score? scoreToSave = Scores?.FirstOrDefault(t => t.ScoreId == 0);
            if (scoreToSave != null)
            {
                scoreToSave.UserName = NewUserName;
                await GameDataSaver.AddScore(scoreToSave);
                KeyboardProvider.Disable(true);
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }

        }
    }
    public void Dispose()
    {
        KeyboardProvider.Disable(true);
        KeyboardProvider.Reset();

        if (Subscriptions != null)
        {
            Subscriptions.Dispose();
        }

    }
}
