using BAP.Db;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BAP.Helpers.BapBasicGameHelper;
using BAP.Types;
using BAP.Helpers;
using BAP.MemoryGames.Components;
using MessagePipe;

namespace BAP.MemoryGames
{


    public class MadQuickCatDescription : IBapGameDescription
    {
        public Type TypeOfInitialDisplayComponent => typeof(Simon);

        public string Name => "Mad quickCat ";
        public string Description => "A Memory game that gets a little harder each time.";
        public string UniqueId => "0bb0b440-0497-4570-8df4-0e5a91239619";
        public string ScoringModelVersion => "1.0.0";
    }

    public class SimonGame : IBapGame, IDisposable
    {
        private ILogger<SimonGame> _logger { get; set; }
        //public IGameDataSaver<MadQuickCatDescription> DbSaver { get; set; }
        private ILayoutHandler LayoutHandler { get; set; }
        public bool IsGameRunning { get; internal set; }
        ISubscriber<ButtonPressedMessage> ButtonPressedMessages { get; set; } = default!;
        IDisposable subscriptions = default!;
        IBapMessageSender MsgSender { get; set; } = default!;
        public int RoundsCompleted
        {
            get
            {
                return ButtonOrder.Count - 1;
            }
        }
        private Dictionary<string, (BapColor color, string soundFileName)> ButtonAssignments = new();
        private List<string> ButtonOrder = new List<string>();
        private List<string> buttonSounds = new List<string>();
        private int currentButtonNumber;
        public int MaxButtonCount
        {
            get
            {
                return buttonSounds.Count;
            }
        }

        int delay = 10;
        bool playingPattern = false;
        List<string> buttonsInUse = new List<string>();
        private static System.Timers.Timer gameTimer = default!;
        public SimonGame(IBapMessageSender messageSender, ILogger<SimonGame> logger, ISubscriber<ButtonPressedMessage> buttonPressedMessages, ILayoutHandler layoutHandler)
        {
            _logger = logger;
            ButtonPressedMessages = buttonPressedMessages;
            MsgSender = messageSender;
            var bag = DisposableBag.CreateBuilder();
            ButtonPressedMessages.Subscribe(async (x) => await OnButtonPressed(x)).AddTo(bag);
            subscriptions = bag.Build();
            LayoutHandler = layoutHandler;
            //DbSaver = dbSaver;
            if (buttonSounds.Count == 0)
            {
                buttonSounds.Add("db4.mp3");
                buttonSounds.Add("d4.mp3");
                buttonSounds.Add("e4.mp3");
                buttonSounds.Add("g4.mp3");
                buttonSounds.Add("a4.mp3");
                buttonSounds.Add("bb4.mp3");
                buttonSounds.Add("b4.mp3");
            }
        }

        public async Task<bool> ForceEndGame()
        {
            return await EndGame("Game was force Closed", true);
        }



        public void SetupGame(int buttonCountToUse, int rowIdToStartWith)
        {
            if (LayoutHandler.CurrentButtonLayout != null && rowIdToStartWith > 0)
            {
                buttonsInUse = LayoutHandler.CurrentButtonLayout.ButtonPositions.Where(t => t.RowId >= rowIdToStartWith).OrderBy(t => t.RowId).ThenBy(t => t.ColumnId).Take(buttonCountToUse).Select(t => t.ButtonId).ToList();
            }
            else
            {
                buttonsInUse = MsgSender.GetConnectedButtonsInOrder().Take(buttonCountToUse).ToList();
            }
        }

        internal (string shortVersion, string longVersion) GetDifficulty(int buttonCount)
        {
            return buttonCount switch
            {
                <= 3 => ("1", "Easy"),
                > 6 => ("10", "Hard"),
                _ => ("5", "Medium")
            };
        }

        internal Score GenerateScoreWithCurrentData()
        {

            var buttonDifficulty = GetDifficulty(buttonsInUse.Count);
            int roundsCompleted = RoundsCompleted;
            Score score = new Score()
            {
                DifficultyName = buttonDifficulty.shortVersion,
                DifficultyDescription = $"{buttonDifficulty.longVersion}",
                ScoreData = $"{buttonsInUse.Count}|{ButtonOrder.Count}",
                NormalizedScore = ButtonOrder.Count,
                ScoreDescription = $"Completed {roundsCompleted} rounds"
            };
            return score;
        }

        public async Task<bool> Start()
        {
            IsGameRunning = true;

            if (gameTimer != null)
            {
                gameTimer.Dispose();
            }
            gameTimer = new System.Timers.Timer(delay * 1000);
            gameTimer.Elapsed += EndGameEvent;
            gameTimer.AutoReset = false;
            ButtonOrder = new List<string>();

            if (buttonsInUse.Count == 0)
            {
                buttonsInUse = MsgSender.GetConnectedButtonsInOrder();
            }


            int buttonCount = buttonsInUse.Count;

            if (buttonCount < 2)
            {
                _logger.LogDebug($"There are {buttonCount} buttons connected");
                MsgSender.SendUpdate($"Only {buttonCount} buttons. You need 2 buttons or more to play", true, fatalError: true);
                IsGameRunning = false;
                return false;
            }
            int currentSoundCount = 0;
            MsgSender.SendImageToAllButtons(new ButtonImage());
            if (ButtonAssignments.Count != buttonCount)
            {
                ButtonAssignments = new();
                foreach (var button in buttonsInUse)
                {
                    var rgb = StandardColorPalettes.Default[currentSoundCount];
                    ButtonAssignments.Add(button, (rgb, buttonSounds[currentSoundCount]));
                    currentSoundCount++;
                }
            }

            await UpdateAndPlayThePattern();
            return true;
        }
        private void EndGameEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            _ = EndGame("Time Expired - Game Ended");
        }
        public async Task<bool> EndGame(string message, bool forcedClosed = false)
        {

            _logger.LogDebug(message);
            IsGameRunning = false;
            gameTimer?.Stop();
            //play the error sound
            bool highScore = false;
            if (forcedClosed)
            {
                return false;
            }
            highScore = false;//(await DbSaver.GetScoresWithNewScoreIfWarranted(GenerateScoreWithCurrentData())).Where(t => t.ScoreId == 0).Any();
            if (highScore)
            {

                MsgSender.SendImageToAllButtons(new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.AllOneColor), new BapColor(0, 255, 0)));
            }
            else
            { //todo a timout of 6000 was lost here. 
                MsgSender.SendImageToAllButtons(new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.AllOneColor), new BapColor(255, 0, 0)));
            }
            MsgSender.SendUpdate($" You completed {RoundsCompleted} successful rounds", true, highScore);

            return true;
        }
        private async Task<bool> NextCommand()
        {
            if (currentButtonNumber != 0)
            {
                //play the good sound;
            }
            if (currentButtonNumber == ButtonOrder.Count - 1)
            {
                _logger.LogDebug($"Round Finished");
                gameTimer.Stop();
                playingPattern = true;
                await Task.Delay(500);
                MsgSender.PlayAudio("simonroundcomplete.mp3");
                await Task.Delay(1500);
                await UpdateAndPlayThePattern();
            }
            else
            {
                _logger.LogDebug("Moving to the next Button");
                gameTimer.Stop();
                gameTimer.Start();
                currentButtonNumber++;
            }
            return true;
        }

        private async Task<bool> UpdateAndPlayThePattern()
        {
            if (IsGameRunning)
            {
                if (ButtonOrder.Count == 0)
                {
                    SendColorsToAllButtons();
                    await Task.Delay(1000);
                }

                playingPattern = true;
                currentButtonNumber = 0;
                string lastNodeId = ButtonOrder.Count > 0 ? ButtonOrder.Last() : "";
                string nodeId = GetRandomNodeId(buttonsInUse, lastNodeId, 1);
                ButtonOrder.Add(nodeId);
                MsgSender.SendUpdate($"You have completed {ButtonOrder.Count} rounds", false);
                _logger.LogDebug($"Sending the pattern with {ButtonOrder.Count} items in the list");
                foreach (var button in ButtonOrder)
                {
                    _logger.LogDebug($"Sending command to button {button}");
                    if (IsGameRunning)
                    {
                        var (color, soundFileName) = ButtonAssignments[button];
                        MsgSender.PlayAudio(soundFileName);
                        //todo - lost the brightness effect
                        MsgSender.SendImage(button, new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.AllOneColor), color));
                        await Task.Delay(750);
                    }
                }
                gameTimer.Start();
                playingPattern = false;

            }
            _logger.LogDebug($"Sending the static images to all of the buttons");
            return true;
        }

        private void SendColorsToAllButtons()
        {
            foreach (var fullButton in ButtonAssignments)
            {

                if (IsGameRunning)
                {
                    var (color, _) = fullButton.Value;
                    //Todo - Some timeout stuff has been lost
                    MsgSender.SendImage(fullButton.Key, new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.AllOneColor), color));

                }
            }
        }

        async Task OnButtonPressed(ButtonPressedMessage e)
        {
            if (IsGameRunning)
            {
                if (playingPattern)
                {
                    EndGame("Button pressed while the pattern is playing");
                }
                else
                {
                    string currentButton = ButtonOrder[currentButtonNumber];
                    if (ButtonAssignments.TryGetValue(e.NodeId, out var colorAndFile))
                    {

                        MsgSender.PlayAudio(colorAndFile.soundFileName);
                        if (currentButton == e.NodeId)
                        {
                            _logger.LogDebug($"Correct moving to the next item.");
                            await NextCommand();
                        }
                        else
                        {
                            EndGame("Wrong button");
                        }
                    }

                }

            }


        }

        public void Dispose()
        {
            if (subscriptions != null)
            {
                subscriptions.Dispose();
            }
            gameTimer?.Dispose();
        }

    }
}
