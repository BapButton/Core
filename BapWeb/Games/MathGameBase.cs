using BapDb;
using FluentValidation.Validators;
using MessagePipe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BapButton;
using BapShared;

namespace BapWeb.Games
{
    public abstract class MathGameBase : ITinkerGame, IDisposable
    {
        public abstract ILogger _logger { get; set; }
        public bool IsGameRunning { get; internal set; }
        public abstract IGameDataSaver DbSaver { get; internal set; }
        ISubscriber<KeyboardKeyPressedMessage> KeyPressed { get; set; } = default!;
        GameHandler GameHandler { get; set; } = default!;
        IDisposable subscriptions = default!;
        internal BapColor numberColor = new BapColor(0, 255, 0);
        internal string lastNodeId = "";
        internal int correctScore = 0;
        internal int wrongScore = 0;
        internal List<string> nodeIdsToUseForDisplay { get; set; } = new();

        internal int SecondsToRun { get; set; }
        internal MessageSender MsgSender { get; set; }
        internal KeyboardHandler KeyboardHandler { get; set; }
        //private NumberKeyboard _numberKeyboard { get; set; } = default!;
        internal PausableTimer gameTimer = new PausableTimer();
        internal char[] Answer { get; set; } = Array.Empty<char>();
        DateTime gameEndTime;
        List<string> _correctSounds = new();
        List<string> _wrongSounds = new();
        //List<string> buttonsInOrder = new();
        public int CurrentSpotInAnswerString = 0;
        internal int ButtonCount { get; set; } = 0;
        internal char CurrentDigit => Answer[CurrentSpotInAnswerString];

        DefaultKeyboard keyboard { get; set; }



        internal MathGameBase(KeyboardHandler keyboardHandler, GameHandler gameHandler, MessageSender msgSender, ISubscriber<KeyboardKeyPressedMessage> keyPressed)
        {
            KeyboardHandler = keyboardHandler;
            MsgSender = msgSender;
            KeyPressed = keyPressed;
            GameHandler = gameHandler;
            keyboard = keyboardHandler.SetNewKeyboard<DefaultKeyboard>(true)!;
            var bag = DisposableBag.CreateBuilder();
            KeyPressed.Subscribe(async (x) => await OnCharacterPressed(x)).AddTo(bag);
            subscriptions = bag.Build();
        }

        public MathGameStatus GetStatus()
        {
            return new MathGameStatus()
            {
                CorrectScore = correctScore,
                WrongScore = wrongScore,
                TimeRemaining = gameEndTime - DateTime.Now
            };

        }

        public void UpdateCorrectNumber(char correctCharacter, bool wasLastPressCorrect)
        {

            keyboard.UpdateCorrectValue(correctCharacter, wasLastPressCorrect, new ButtonDisplay(numberColor, Patterns.CheckMark));
        }

        public virtual async void EndGameEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            keyboard.Disable();
            IsGameRunning = true;
            Score score = GenerateScoreWithCurrentData();
            List<Score> scores = await DbSaver.GetScoresWithNewScoreIfWarranted(score);
            await EndGame("Time Expired - Game Ended", scores.Where(t => t.ScoreId == 0).Any(), false);
        }

        internal abstract Score GenerateScoreWithCurrentData();

        public virtual async Task<bool> EndGame(string message, bool highScoreAchieved = false, bool forceClosed = true)
        {
            IsGameRunning = false;
            _logger.LogInformation(message);
            gameTimer.Dispose();
            keyboard.Disable(!highScoreAchieved);
            if (forceClosed)
            {
                ButtonDisplay standardButtonMessage = new();
                MsgSender.SendGeneralCommand(new StandardButtonCommand(standardButtonMessage));
            }
            else if (forceClosed == false && !highScoreAchieved)
            {
                ButtonDisplay standardButtonMessage = new(255, 0, 0, turnOffAfterMillis: 3000);
                MsgSender.SendGeneralCommand(new StandardButtonCommand(standardButtonMessage, standardButtonMessage));
                MsgSender.PlayAudio("GameFailure.mp3");
            }
            else
            {
                ButtonDisplay standardButtonMessage = new(0, 0, 255, turnOffAfterMillis: 3000);
                MsgSender.SendGeneralCommand(new StandardButtonCommand(standardButtonMessage, standardButtonMessage));
                MsgSender.PlayAudio("GameSuccess.mp3");
                await Task.Delay(2000);
            }
            MsgSender.SendUpdate("Game Ended", true, highScoreAchieved);
            return true;
        }


        public virtual string GetNextSound(List<string> files)
        {
            return TinkerButtonGameMethods.GetRandomNodeId(files);
        }

        public virtual void PlayNextWrongSound()
        {
            MsgSender.PlayAudio(GetNextSound(_wrongSounds), true);
        }
        public virtual Task<bool> WrongButtonPressed(bool setupNextMathProblem)
        {
            wrongScore++;
            PlayNextWrongSound();
            if (setupNextMathProblem)
            {
                SetupNextMathProblem(false);
            }
            return Task.FromResult(true);
        }

        public virtual bool RightButtonPressed()
        {
            MsgSender.SendUpdate("Score Updated");
            return true;
        }

        public virtual async Task CorrectMathResultAchievedAsync(bool setupNextMathProblem)
        {
            MsgSender.PlayAudio(GetNextSound(_correctSounds));
            correctScore++;
            if (setupNextMathProblem)
            {
                var nextProblemCreated = await SetupNextMathProblem(true);
                if (!nextProblemCreated)
                {
                    await EndGame("Game Completed");
                }
            }

        }

        public abstract Task<bool> SetupNextMathProblem(bool wasLastPressCorrect);

        public async virtual Task OnCharacterPressed(KeyboardKeyPressedMessage e)
        {
            await NumberPressed(e.KeyValue);
        }

        public virtual async Task<bool> NumberPressed(char digit)
        {
            if (IsGameRunning)
            {
                if (CurrentDigit == digit)
                {
                    RightButtonPressed();
                    if (CurrentSpotInAnswerString + 1 == Answer.Length)
                    {

                        CorrectMathResultAchievedAsync(true);
                    }
                    else
                    {
                        CurrentSpotInAnswerString++;
                        UpdateCorrectNumber(CurrentDigit, true);
                    }

                    return true;
                }
                else
                {
                    await WrongButtonPressed(true);
                    return false;
                }

            }
            return true;

        }

        public virtual void SetNewAnswer(int answer, bool wasLastPressCorrect)
        {
            SetNewAnswer(answer.ToString().ToCharArray(), wasLastPressCorrect);
        }

        public virtual void SetNewAnswer(char[] answer, bool wasLastPressCorrect)
        {
            Answer = answer;
            CurrentSpotInAnswerString = 0;
            UpdateCorrectNumber(Answer[0], wasLastPressCorrect);
        }


        internal virtual void Initialize(List<string>? correctSounds = null, List<string>? wrongSounds = null)
        {
            if (correctSounds == null)
            {
                _correctSounds.Add("open_001.wav");
            }
            else
            {
                _correctSounds = correctSounds;
            }
            if (wrongSounds == null)
            {
                _wrongSounds.Add("error_004.wav");
            }
            else
            {
                _wrongSounds = wrongSounds;
            }
        }
        public async virtual Task<bool> Start()
        {
            return await StartGame(120, false);
        }

        public virtual bool SetButtonCountAndButtonList(bool useTopRowForDisplay)
        {
            if (useTopRowForDisplay)
            {
                ButtonCount = GameHandler?.CurrentButtonLayout?.ButtonPositions.Where(t => t.RowId != 1)?.Count() ?? MsgSender.GetConnectedButtons().Count;
                nodeIdsToUseForDisplay = GameHandler?.CurrentButtonLayout?.ButtonPositions.Where(t => t.RowId == 1).Select(t => t.ButtonId).ToList() ?? new List<string>(); ;
                if (nodeIdsToUseForDisplay.Count < 3)
                {
                    nodeIdsToUseForDisplay = new List<string>();
                    useTopRowForDisplay = false;
                }
            }
            if (!useTopRowForDisplay)
            {
                ButtonCount = MsgSender.GetConnectedButtons().Count;
                nodeIdsToUseForDisplay = new();
            }

            return true;
        }

        public async virtual Task<bool> StartGame(int secondsToRun, bool useTopRowForDisplay, KeyboardType keyboardType = KeyboardType.Numbers)
        {
            if (IsGameRunning)
            {
                return false;
            }
            IsGameRunning = true;
            SecondsToRun = secondsToRun; ;
            MsgSender.SendUpdate("Game Started");
            gameTimer = new PausableTimer(secondsToRun * 1000);
            gameTimer.Elapsed += EndGameEvent;
            gameTimer.AutoReset = false;
            gameTimer.Start();
            gameEndTime = DateTime.Now.AddSeconds(secondsToRun);
            correctScore = 0;
            wrongScore = 0;
            nodeIdsToUseForDisplay = new List<string>();
            SetButtonCountAndButtonList(useTopRowForDisplay);

            keyboard = KeyboardHandler.SetNewKeyboard<DefaultKeyboard>(true);
            keyboard.SetupKeyboard(keyboardType, nodesToAvoid: nodeIdsToUseForDisplay, alwaysUseAllButtons: true, playKeyPressSound: true);
            if (ButtonCount < 5)
            {
                MsgSender.SendUpdate($"Not enough Buttons. You only have {ButtonCount} buttons but you need at least 5", true);
                await EndGame($"Not enough Buttons.");
                return false;
            }
            if (GameHandler?.CurrentButtonLayout == null)
            {
                MsgSender.SendUpdate($"No layout setup. Setup a layout for your buttons", true);
                await EndGame($"No layout setup.");
                return false;
            }
            MsgSender.SendGeneralCommand(new StandardButtonCommand(new ButtonDisplay(0, 0, 0)));
            MsgSender.SendUpdate("Starting Game", false);
            keyboard.ShowKeyboard();
            await SetupNextMathProblem(false);
            return true;
        }

        public async Task<bool> ForceEndGame()
        {
            await EndGame("Game Force Ended");
            return true;
        }

        public void Dispose()
        {
            if (subscriptions != null)
            {
                subscriptions.Dispose();
            }

        }
    }
    public class MathGameStatus
    {
        public int CorrectScore { get; set; }
        public int WrongScore { get; set; }
        public TimeSpan TimeRemaining { get; set; }
        public int QuestionsRemaining { get; set; }
        public TimeSpan TimeSinceStartOfGame { get; set; }
    }
}

