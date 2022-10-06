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
    public class MultiplicationGameDescription : IGameDescription
    {
        public Type TypeOfInitialDisplayComponent => typeof(Multiplication);
        public string Name => "Multiplication Game";
        public string Description => "Multiplication Game with Adjustable Difficulty to practice quickly doing addition facts.";

        public string UniqueId => "6356fc9d-abca-479e-9d0b-3519517ae169";
        public string ScoringModelVersion => "1.0.1";
    }
    public class MultiplicationGame : MathGameBase
    {
        public override ILogger _logger { get; set; }
        public override IGameDataSaver DbSaver { get; internal set; }
        public int Multiplicand = -1;
        public int Multiplier = -1;
        public int MinNumber { get; set; } = 0;
        public int MaxNumber { get; set; } = 10;
        public int MaxValue { get; set; } = 100;
        public bool IsSpanish { get; set; } = false;



        public MultiplicationGame(KeyboardHandler keyboardHandler, GameHandler gameHandler, ILogger<AdditionGame> logger, ISubscriber<KeyboardKeyPressedMessage> keyPressed, MessageSender messageSender, IGameDataSaver<AdditionGameDescription> dbSaver) : base(keyboardHandler, gameHandler, messageSender, keyPressed)
        {
            _logger = logger;
            DbSaver = dbSaver;
        }


        public override async Task<bool> SetupNextMathProblem(bool wasLastPressCorrect)
        {
            int loopCount = 0;
            do
            {
                Multiplicand = GetRandomInt(MinNumber, MaxNumber);
                Multiplier = GetRandomInt(MinNumber, MaxNumber);
                loopCount++;
            }
            while (Multiplicand * Multiplier > MaxValue && loopCount < 100);


            CurrentSpotInAnswerString = 0;
            if (nodeIdsToUseForDisplay.Count > 2)
            {

                ButtonDisplay buttonDisplay = new(new BapColor(0, 255, 0), EnumHelper.GetPatternFromNumber(Multiplicand));
                MsgSender.SendCommand(nodeIdsToUseForDisplay[0], new StandardButtonCommand() { InitialDisplay = buttonDisplay, OnPressDisplay = buttonDisplay });
                buttonDisplay = new(new BapColor(0, 255, 0), Patterns.PlusSign);
                MsgSender.SendCommand(nodeIdsToUseForDisplay[1], new StandardButtonCommand() { InitialDisplay = buttonDisplay, OnPressDisplay = buttonDisplay });
                buttonDisplay = new(new BapColor(0, 255, 0), EnumHelper.GetPatternFromNumber(Multiplier));
                MsgSender.SendCommand(nodeIdsToUseForDisplay[2], new StandardButtonCommand() { InitialDisplay = buttonDisplay, OnPressDisplay = buttonDisplay });
            }
            SetNewAnswer((Multiplicand * Multiplier), wasLastPressCorrect);
            MsgSender.SendUpdate("Next math problem Added");

            string filename = IsSpanish ? $"{Multiplicand} por {Multiplier}" : $"{Multiplicand} times {Multiplier}";
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
            var (shortVersion, longVersion) = GetFullDifficultInfo(MinNumber, MaxNumber, ButtonCount);
            decimal normalizedScore = 0;
            if (SecondsToRun > 0 && correctScore + wrongScore != 0)
            {
                normalizedScore = ((decimal)correctScore - ((decimal)wrongScore * 2.0M)) / ((decimal)SecondsToRun / 30.0M);
            }
            Score score = new Score()
            {
                Difficulty = shortVersion,
                DifficultyDetails = longVersion,
                ScoreData = $"{correctScore}|{wrongScore}|{MinNumber}|{MaxNumber}|{MaxValue}|{SecondsToRun}",
                NormalizedScore = normalizedScore,
                ScoreDescription = $"{correctScore} correct and {wrongScore} wrong while playing for {timeSpan.Minutes}:{timeSpan.Seconds:D2}",
                ScoreFullDetails = $"{correctScore} correct and {wrongScore} wrong while playing for {timeSpan.Minutes}:{timeSpan.Seconds:D2} with values between {MinNumber} and {MaxValue} with a max of {MaxValue} and {ButtonCount} buttons"
            };
            return score;
        }
    }

}


