using BAP.BasicMathGames.Components;
using static BAP.Helpers.BapBasicGameHelper;

namespace BAP.BasicMathGames
{

    public class AdditionGame : KeyboardGameBase
    {
        public override ILogger _logger { get; set; }
        public override IGameDataSaver DbSaver { get; set; }
        public int AddendOne = -1;
        public int AddendTwo = -1;
        public int MinAddend { get; set; } = 0;
        public int MaxAddend { get; set; } = 10;
        public int MaxValue { get; set; } = 100;
        public bool IsSpanish { get; set; } = false;

        public AdditionGame(ILayoutProvider layoutProvider, IKeyboardProvider keyboardProvider, IGameHandler gameHandler, ILogger<AdditionGame> logger, ISubscriber<KeyboardKeyPressedMessage> keyPressed, IBapMessageSender messageSender, IGameDataSaver dbSaver) : base(keyboardProvider, gameHandler, layoutProvider, messageSender, keyPressed)
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

                BapColor textColor = new BapColor(0, 255, 0);
                MsgSender.SendImage(nodeIdsToUseForDisplay[0], new(PatternHelper.GetBytesForPattern(EnumHelper.GetPatternFromNumber(AddendOne)), textColor));

                MsgSender.SendImage(nodeIdsToUseForDisplay[1], new(PatternHelper.GetBytesForPattern(Patterns.PlusSign), textColor));
                MsgSender.SendImage(nodeIdsToUseForDisplay[0], new(PatternHelper.GetBytesForPattern(EnumHelper.GetPatternFromNumber(AddendTwo)), textColor));
            }
            SetNewAnswer((AddendOne + AddendTwo), wasLastPressCorrect);
            MsgSender.SendUpdate("Next math problem Added");

            string filename = IsSpanish ? $"{AddendOne} mas {AddendTwo}" : $"{AddendOne} plus {AddendTwo}";
            if (IsSpanish)
            {
                MsgSender.PlayTTS(filename, true, TTSLanguage.Spanish);
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

        public override Score GenerateScoreWithCurrentData()
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
                DifficultyId = shortVersion,
                DifficultyDescription = longVersion,
                ScoreData = $"{correctScore}|{wrongScore}|{MinAddend}|{MaxAddend}|{MaxValue}|{SecondsToRun}",
                NormalizedScore = normalizedScore,
                ScoreDescription = $"{correctScore} correct and {wrongScore} wrong while playing for {timeSpan.Minutes}:{timeSpan.Seconds:D2}",
            };
            return score;
        }
    }

}


