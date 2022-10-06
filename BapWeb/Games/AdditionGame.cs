using BapDb;
using MessagePipe;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Timers;
using BapWeb.Pages;
using BapButton;
using BapShared;
using static BapButton.TinkerButtonGameMethods;

namespace BapWeb.Games
{
    public class AdditionGameDescription : IGameDescription
    {
        public Type TypeOfInitialDisplayComponent => typeof(Addition);
        public string Name => "Addition Game";
        public string Description => "Addition Game with Adjustable Difficulty to practice quickly doing addition facts.";

        public string UniqueId => "57c74fd7-19c3-4de3-9af9-71aeb466ef1a";
        public string ScoringModelVersion => "1.0.1";
    }
    public class AdditionGame : MathGameBase
    {
        public override ILogger _logger { get; set; }
        public override IGameDataSaver DbSaver { get; internal set; }
        public int AddendOne = -1;
        public int AddendTwo = -1;
        public int MinAddend { get; set; } = 0;
        public int MaxAddend { get; set; } = 10;
        public int MaxValue { get; set; } = 100;
        public bool IsSpanish { get; set; } = false;



        public AdditionGame(KeyboardHandler keyboardHandler, GameHandler gameHandler, ILogger<AdditionGame> logger, ISubscriber<KeyboardKeyPressedMessage> keyPressed, MessageSender messageSender, IGameDataSaver<AdditionGameDescription> dbSaver) : base(keyboardHandler, gameHandler, messageSender, keyPressed)
        {
            _logger = logger;
            DbSaver = dbSaver;
        }


        public override async Task<bool> SetupNextMathProblem(bool wasLastPressCorrect)
        {
            int loopCount = 0;
            do
            {
                AddendOne = GetRandomInt(MinAddend, MaxAddend);
                AddendTwo = GetRandomInt(MinAddend, MaxAddend);
                loopCount++;
            }
            while (AddendOne + AddendTwo > MaxValue && loopCount < 100);


            CurrentSpotInAnswerString = 0;
            if (nodeIdsToUseForDisplay.Count > 2)
            {

                ButtonDisplay buttonDisplay = new(new BapColor(0, 255, 0), EnumHelper.GetPatternFromNumber(AddendOne));
                MsgSender.SendCommand(nodeIdsToUseForDisplay[0], new StandardButtonCommand() { InitialDisplay = buttonDisplay, OnPressDisplay = buttonDisplay });
                buttonDisplay = new(new BapColor(0, 255, 0), Patterns.PlusSign);
                MsgSender.SendCommand(nodeIdsToUseForDisplay[1], new StandardButtonCommand() { InitialDisplay = buttonDisplay, OnPressDisplay = buttonDisplay });
                buttonDisplay = new(new BapColor(0, 255, 0), EnumHelper.GetPatternFromNumber(AddendTwo));
                MsgSender.SendCommand(nodeIdsToUseForDisplay[2], new StandardButtonCommand() { InitialDisplay = buttonDisplay, OnPressDisplay = buttonDisplay });
            }
            SetNewAnswer((AddendOne + AddendTwo), wasLastPressCorrect);
            MsgSender.SendUpdate("Next math problem Added");

            string filename = IsSpanish ? $"{AddendOne} mas {AddendTwo}" : $"{AddendOne} plus {AddendTwo}";
            if (IsSpanish)
            {
                MsgSender.PlayTTSSpanish(filename, true);
            }
            else
            {
                MsgSender.PlayTTS(filename, true);
            }

            return await Task.FromResult(true);
        }

        public (string shortVersion, string longVersion) GetDifficulty(int minAddend, int maxAddend)
        {
            return (minAddend, maxAddend, maxAddend - minAddend) switch
            {
                ( < 21, _, < 21) => ("1", "Easy"),
                (_, _, > 40) => ("10", "Hard"),
                _ => ("5", "Medium")
            };
        }

        private (string shortVersion, string longVersion) ButtonType(int buttonCount)
        {
            return buttonCount switch
            {
                < 7 => ("<6", "6 or less Buttons"),
                > 9 => ("10+", "Full Buttons"),
                _ => ("7-9", "7-9 Buttons")
            };
        }
        public (string shortVersion, string longVersion) GetFullDifficultInfo(int minaddend, int maxAddend, int buttonCount)
        {
            var problemDifficulty = GetDifficulty(minaddend, maxAddend);
            var buttonDifficulty = ButtonType(buttonCount);
            string displayMessage = nodeIdsToUseForDisplay.Count > 0 ? " using buttons as a display" : "";
            string shortVersion = $"{buttonDifficulty.shortVersion}|{problemDifficulty.shortVersion}|{nodeIdsToUseForDisplay.Count > 0}";
            string longVersion = $"{buttonDifficulty.longVersion} with a difficulty of {problemDifficulty.longVersion}{displayMessage}.";
            return (shortVersion, longVersion);
        }

        internal override Score GenerateScoreWithCurrentData()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(SecondsToRun);
            var (shortVersion, longVersion) = GetFullDifficultInfo(MinAddend, MaxAddend, ButtonCount);
            decimal normalizedScore = 0;
            if (SecondsToRun > 0 && correctScore + wrongScore != 0)
            {
                normalizedScore = ((decimal)correctScore - ((decimal)wrongScore * 2.0M)) / ((decimal)SecondsToRun / 30.0M);
            }
            Score score = new Score()
            {
                Difficulty = shortVersion,
                DifficultyDetails = longVersion,
                ScoreData = $"{correctScore}|{wrongScore}|{MinAddend}|{MaxAddend}|{MaxValue}|{SecondsToRun}",
                NormalizedScore = normalizedScore,
                ScoreDescription = $"{correctScore} correct and {wrongScore} wrong while playing for {timeSpan.Minutes}:{timeSpan.Seconds:D2}",
                ScoreFullDetails = $"{correctScore} correct and {wrongScore} wrong while playing for {timeSpan.Minutes}:{timeSpan.Seconds:D2} with values between {MinAddend} and {MaxValue} with a max of {MaxValue} and {ButtonCount} buttons"
            };
            return score;
        }
    }

}


