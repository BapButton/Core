using MessagePipe;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using BapButton;
using static BAP.Helpers.BapBasicGameHelper;
using BAP.Types;
using BAP.Helpers;
using Microsoft.Extensions.Logging;
using BAP.ReactionGames.Components;

namespace BAP.ReactionGames
{
    public class ReactionGameEthanDescription : IBapGameDescription
    {
        public Type TypeOfInitialDisplayComponent => typeof(ReactionEthan);
        public string Name => "Ethan Sword Game";
        public string Description => "Ethans version of the Sword game with a special bonus round.";
        public string UniqueId => "0fe4493b-714d-465d-8425-f49b3bd91289";
        public string ScoringModelVersion => "1.0.0";
    }
    public class ReactionGameEthan : ReactionGameBase
    {
        string lastFaceNodeId = "";
        string lastSwordNodeId = "";
        bool lastFaceWasAFrownyFace = false;
        int swordGameCount = 0;
        ulong[] swordSprite = new ulong[64];
        ulong[] frownyFace = new ulong[64];
        SwordBonusGame? bonusGame { get; set; } = null;
        private const string FrownyFaceSound = "SgLostTheEntireGame.mp3";
        private const string TooManyWrong = "SgLostTheEntireGame.mp3";
        private const string BeatTheBonusRound = "SqBeatTheBonusRound.mp3";
        private const string StartOfBonusRound = "SgStart.mp3";
        internal override ILogger _logger { get; set; }
        private IServiceProvider Services { get; set; }
        private ISubscriber<InternalSimpleGameUpdates> InternalUpdatePipe { get; set; }
        IDisposable subscriptions = default!;

        public ReactionGameEthan(ILogger<ReactionGameBase> logger, ISubscriber<ButtonPressedMessage> buttonPressed, IBapMessageSender messageSender, IServiceProvider services, ISubscriber<InternalSimpleGameUpdates> internalUpdatePipe) : base(buttonPressed, messageSender)
        {
            Services = services;
            InternalUpdatePipe = internalUpdatePipe;
            base.Initialize(minButtons: 2, useIfItWasLitForScoring: false);
            var bag = DisposableBag.CreateBuilder();
            InternalUpdatePipe.Subscribe(async (x) => await UpdateScoreFromInternalMessage(x)).AddTo(bag);
            subscriptions = bag.Build();
            _logger = logger;
            string path = Path.Combine(".", "wwwroot", "sprites", "Emoji.png");
            SpriteParser spriteParser = new SpriteParser(path);
            swordSprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 2, 9);
            frownyFace = spriteParser.GetSprite(4, 5, 24, 20, 16, 6, 7);
        }

        public async Task UpdateScoreFromInternalMessage(InternalSimpleGameUpdates update)
        {
            base.correctScore += update.CorrectScore;
            base.wrongScore += update.WrongScore;
            if (update.GameEnded)
            {
                base.UnPauseGame();
                bonusGame?.Dispose();
                MsgSender.PlayAudio(BeatTheBonusRound);
                await NextCommand();
            }
            MsgSender.SendUpdate("Internal Score Update");
        }

        public override async Task<bool> Start(int secondsToRun)
        {
            bonusGame = Services.GetRequiredService<SwordBonusGame>();
            await Task.Delay(100);
            bonusGame.Dispose();
            await base.Start(secondsToRun);
            lastFaceNodeId = "";
            return true;
        }


        public async override Task<bool> NextCommand()
        {
            await base.NextCommand();
            bool sendFace = ShouldWePerformARandomAction(3);
            bool shouldWeShowTheSword = ShouldWePerformARandomAction(10);


            if (lastSwordNodeId == lastNodeId)
            {
                lastSwordNodeId = "";
            }

            if (lastFaceNodeId == lastNodeId)
            {
                lastFaceNodeId = "";
            }
            if (shouldWeShowTheSword && swordGameCount < 5)
            {
                swordGameCount++;
                string swordNodeId = GetRandomNodeId(buttons, lastNodeId, 0);
                lastSwordNodeId = swordNodeId;
                MsgSender.SendImage(swordNodeId, new ButtonImage(swordSprite));
            }
            if (sendFace)
            {
                bool sendFrownyFace = ShouldWePerformARandomAction(3);
                string faceNodeId = GetRandomNodeId(buttons, lastNodeId, 0);
                lastFaceNodeId = faceNodeId;

                if (sendFrownyFace)
                {
                    lastFaceWasAFrownyFace = true;
                    MsgSender.SendImage(faceNodeId, new ButtonImage(frownyFace));

                }
                else
                {
                    lastFaceWasAFrownyFace = false;
                    MsgSender.SendImage(faceNodeId, new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.PlainSmilyFace), new BapColor(0, 255, 0)));
                }
            }

            return true;

        }
        public override void Dispose()
        {
            subscriptions.Dispose();
            base.Dispose();
        }

        public bool IsBonusGameRunning
        {
            get
            {
                return bonusGame?.IsGameRunning ?? false;
            }
        }

        public void ForceEndBonusGame()
        {

            bonusGame?.Dispose();

        }
        public async override Task OnButtonPressed(ButtonPressedMessage e)
        {
            if (!GamePaused)
            {
                if (lastSwordNodeId == e.NodeId)
                {
                    if (e.ButtonPress.TimeSinceLightTurnedOff < 150)
                    {
                        if (bonusGame != null)
                        {
                            bonusGame.Dispose();
                        }
                        bonusGame = Services.GetRequiredService<SwordBonusGame>();
                        PauseGame(true);
                        MsgSender.PlayAudio(StartOfBonusRound);
                        await bonusGame.Start(20);
                    }
                    else
                    {
                        _logger.Log(LogLevel.Information, $"The sword was pressed but late. TimeSinceLightTurnedOff was {e.ButtonPress.TimeSinceLightTurnedOff}");

                    }


                }
                else if (lastFaceNodeId == e.NodeId)
                {
                    if (e.ButtonPress.TimeSinceLightTurnedOff < 150)
                    {
                        if (lastFaceWasAFrownyFace)
                        {
                            await EndGame("It was a frowny Face", true);
                            MsgSender.PlayAudio(FrownyFaceSound);
                        }
                        else
                        {
                            //this increments the score by 3 total because rightbuttonpressed does one.;
                            correctScore += 3;
                            await RightButtonPressed(e.ButtonPress, false, false);
                        }
                    }
                    {
                        _logger.Log(LogLevel.Information, $"The smiley was pressed but late. TimeSinceLightTurnedOff was {e.ButtonPress.TimeSinceLightTurnedOff}");
                    }

                }
                else
                {
                    await base.OnButtonPressed(e);
                }
            }

        }


        public override async Task<bool> WrongButtonPressed(ButtonPress bp, bool runNextCommand = false, bool updateScore = true, int amountToAdd = 1)
        {
            wrongScore++;
            base.PlayNextWrongSound();
            if (wrongScore > 5)
            {
                MsgSender.PlayAudio(TooManyWrong);
                await EndGame("Too Many Wrong");
                return false;
            }
            return true;

        }

        public override ButtonImage GenerateNextButton()
        {
            return new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.AllOneColor), new BapColor(GetRandomInt(0, 255), GetRandomInt(0, 255), GetRandomInt(0, 255)));
        }
    }

}
