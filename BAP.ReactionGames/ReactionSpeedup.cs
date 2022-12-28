using BAP.Db;
using BapButton;
using BAP.Types;
using BAP.Helpers;
using BAP.ReactionGames.Components;

namespace BAP.ReactionGames
{
    public class ReactionSpeedup : ReactionGameBase
    {
        internal override ILogger _logger { get; set; }
        internal BapColor colorToUse { get; set; }
        //I think I have thread safety issues here. 
        HashSet<string> NodeIdsCurrentlyShowingSomething { get; set; } = new();
        internal int IncorrectPress { get; set; }
        PausableTimer NextButtonTimer { get; set; }
        public DateTime GameStartedAt { get; set; }
        public DateTime GameEndedAt { get; set; }
        public IGameDataSaver DbSaver { get; set; }
        public TimeSpan GameLength
        {
            get
            {
                if (GameStartedAt == DateTime.MinValue)
                {
                    return TimeSpan.FromSeconds(0);
                }
                if (IsGameRunning)
                {
                    return DateTime.Now - GameStartedAt;
                }

                if (GameEndedAt <= GameStartedAt)
                {
                    return DateTime.Now - GameStartedAt;
                }
                return GameEndedAt - GameStartedAt;
            }
        }

        public string GameLengthDisplay
        {
            get
            {
                return GameLength.ToString("mm\\:ss");
            }
        }

        public int CurrentMillisSpeed { get; set; }

        private int SubtractionInterval
        {
            get
            {

                return CurrentMillisSpeed switch
                {
                    > 1500 => 100,
                    > 1000 => 50,
                    > 500 => 15,
                    > 250 => 5,
                    > 150 => 2,
                    _ => 1,
                };
            }
        }

        public ReactionSpeedup(IGameDataSaver dbSaver, ISubscriber<ButtonPressedMessage> buttonPressed, ILogger<ReactionGame> logger, IBapMessageSender messageSender) : base(buttonPressed, messageSender)
        {
            _logger = logger;
            colorToUse = StandardColorPalettes.Default[1];
            DbSaver = dbSaver;
            NextButtonTimer = new PausableTimer();
            base.Initialize(minButtons: 3);
        }

        public override async Task<bool> Start()
        {
            GameStartedAt = DateTime.Now;
            CurrentMillisSpeed = 2000;
            NextButtonTimer = new PausableTimer(1000);
            NextButtonTimer.Elapsed += TimeForNextButton;
            NextButtonTimer.Start();
            NodeIdsCurrentlyShowingSomething = new();
            return await base.Start(0);
        }

        public async void TimeForNextButton(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (IsGameRunning)
            {
                if (wrongScore > 5)
                {
                    GameEndedAt = DateTime.Now;
                    await EndSpeedupGame("To many wrong presses", true);
                }
                else
                {
                    CurrentMillisSpeed -= SubtractionInterval;
                    string nextNodeId = BapBasicGameHelper.GetRandomNodeId(buttons.Except(NodeIdsCurrentlyShowingSomething).ToList());
                    NodeIdsCurrentlyShowingSomething.Add(nextNodeId);
                    if (buttons.Count == NodeIdsCurrentlyShowingSomething.Count)
                    {
                        GameEndedAt = DateTime.Now;
                        IsGameRunning = false;
                        MsgSender.SendImage(nextNodeId, GenerateNextButton());
                        await Task.Delay(300);
                        await EndSpeedupGame("All of the nodes are showing something", true);
                    }
                    else
                    {
                        NextButtonTimer = new PausableTimer(CurrentMillisSpeed);
                        NextButtonTimer.Elapsed += TimeForNextButton;
                        NextButtonTimer.Start();

                        MsgSender.SendImage(nextNodeId, GenerateNextButton());
                    }
                }

            }



        }

        public async Task<bool> EndSpeedupGame(string message, bool isFailure = false)
        {
            NextButtonTimer.Stop();
            return await EndGame(message, isFailure);
        }

        public override Task<bool> NextCommand()
        {
            return Task.FromResult(true);
        }

        public override Task OnButtonPressed(ButtonPressedMessage e)
        {
            if (IsGameRunning)
            {
                if (NodeIdsCurrentlyShowingSomething.Contains(e.NodeId))
                {
                    NodeIdsCurrentlyShowingSomething.Remove(e.NodeId);
                }
                else
                {
                    WrongButtonPressed(e.ButtonPress, false);
                }
            }

            return Task.FromResult(true);
        }

        public (string shortVersion, string longVersion) GetFullDifficulty(int buttonCount)
        {
            return buttonCount switch
            {
                < 5 => ("1", "Small"),
                >= 10 => ("10", "Large"),
                _ => ("5", "Medium")
            };

        }

        public override async Task<bool> EndGame(string message, bool isFailure = false)
        {
            IsGameRunning = false;
            _logger.LogInformation(message);
            gameTimer?.Dispose();
            bool isHighScore = false;
            if (isFailure)
            {
                isHighScore = (await DbSaver.GetScoresWithNewScoreIfWarranted(GenerateScoreWithCurrentData())).Where(t => t.ScoreId == 0).Any();
            }
            if (isHighScore)
            {

                MsgSender.SendImageToAllButtons(new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.AllOneColor), new(0, 255, 0)));
                await Task.Delay(3000);
                MsgSender.SendImageToAllButtons(new ButtonImage());
                MsgSender.SendUpdate("Game Ended", true, true);
            }
            else
            {
                MsgSender.SendImageToAllButtons(new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.AllOneColor), new(255, 0, 0)));
                await Task.Delay(3000);
                MsgSender.SendImageToAllButtons(new ButtonImage());
                MsgSender.SendUpdate("Game Ended", true);
            }
            return true;
        }

        internal Score GenerateScoreWithCurrentData()
        {
            int buttonCount = buttons?.Count ?? 0;
            var buttonDifficulty = GetFullDifficulty(buttons?.Count ?? 0);
            decimal totalSeconds = (decimal)(GameEndedAt - GameStartedAt).TotalSeconds;
            string timeSpanString = (GameEndedAt - GameStartedAt).ToString("mm\\:ss");
            Score score = new Score()
            {
                DifficultyId = buttonDifficulty.shortVersion,
                DifficultyDescription = $"{buttonDifficulty.longVersion}",
                ScoreData = $"{buttonCount}",
                NormalizedScore = totalSeconds,
                ScoreDescription = $"Kept buttons clear for {timeSpanString}"
            };
            return score;
        }

        public override ButtonImage GenerateNextButton()
        {
            return new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.AllOneColor), colorToUse);
        }

        public override void Dispose()
        {
            NextButtonTimer.Dispose();
            base.Dispose();
        }
    }
}
