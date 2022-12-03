using MessagePipe;
using Microsoft.Extensions.Logging;
using System;
using static BAP.Helpers.BapBasicGameHelper;
using BAP.ReactionGames.Components;

namespace BAP.ReactionGames
{

    public class ReactionGameDescription
    {
        public Type TypeOfInitialDisplayComponent => typeof(Reaction);
        public string Name => "Reaction Game";
        public string Description => "Basic reaction game to see how many lights you can hit in a given amount of time.";
        public string UniqueId => "21bb7960-90d1-476e-8b1e-4a08ec082551";
        public string ScoringModelVersion => "1.0.0";
    }
    [GamePage("Basic reaction game to see how many lights you can hit in a given amount of time.", "Reaction Game")]
    public class ReactionGame : ReactionGameBase
    {
        internal override ILogger _logger { get; set; }

        public ReactionGame(ISubscriber<ButtonPressedMessage> buttonPressed, ILogger<ReactionGame> logger, IBapMessageSender messageSender) : base(buttonPressed, messageSender)
        {
            _logger = logger;
            base.Initialize(minButtons: 3);
        }

        public override ButtonImage GenerateNextButton()
        {
            return new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.AllOneColor), new BapColor(GetRandomInt(0, 255), GetRandomInt(0, 255), GetRandomInt(0, 255)));
        }
    }

}
