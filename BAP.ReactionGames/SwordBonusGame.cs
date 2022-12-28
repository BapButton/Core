using MessagePipe;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BapButton;
using BAP.Types;
using BAP.Helpers;

namespace BAP.ReactionGames
{
    public enum SBGCustomImages
    {
        Sword = 0,
        Crown = 1
    }

    public class SwordBonusGame : ReactionGameBase
    {
        internal override ILogger _logger { get; set; }
        private const string LostBonusRound = "SgLost.wav";
        private const string LostEntireGame = "SgLostTheEntireGame.wav";

        private const string StartOfEntireGame = "SgStartOfEntireGame.mp3";
        private const string WonEntireGame = "SgWonTheEntireGame.wav";
        private const string BeatTheBonusRound = "SqBeatTheBonusRound.wav";
        private const string HitTheCrown = "SqHitTheCrown.mp3";
        private ulong[] SwordSprite { get; set; } = new ulong[64];
        private ulong[] CrownSprite { get; set; } = new ulong[64];
        private int MsDelay { get; set; } = 1500;
        private int CrownDelay { get; set; } = 500;
        //This is just the super short timer that sends out the next button command. 
        private static System.Timers.Timer bonusTimer;

        private IPublisher<InternalSimpleGameUpdates> InternalGamePipe { get; set; } = default!;
        private string CrownNodeId;
        public SwordBonusGame(ILogger<SwordBonusGame> logger, IPublisher<InternalSimpleGameUpdates> internalGameUpdates, ISubscriber<ButtonPressedMessage> buttonPressed, IBapMessageSender messageSender) : base(buttonPressed, messageSender)
        {
            InternalGamePipe = internalGameUpdates;
            CrownNodeId = "";
            _logger = logger;

            string path = Path.Combine(".", "wwwroot", "sprites", "SwordBonusGame.bmp");
            SpriteParser spriteParser = new SpriteParser(path);
            var sprites = spriteParser.GetCustomImagesFromCustomSprite();
            SwordSprite = sprites[0];
            CrownSprite = sprites[1];
            base.Initialize(minButtons: 2, useIfItWasLitForScoring: true);

        }

        public override ButtonImage GenerateNextButton()
        {
            return new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.AllOneColor), new BapColor(255, 120, 120));
        }

        private void BonusGameEnded()
        {
            InternalGamePipe.Publish(new InternalSimpleGameUpdates(0, 0, true, ""));
        }
        public override async Task<bool> RightButtonPressed(ButtonPress bp, bool runNextCommand = true, bool updateScore = true, int amountToAdd = 1)
        {

            await base.RightButtonPressed(bp, true, false);
            InternalGamePipe.Publish(new InternalSimpleGameUpdates(2, 0, false, ""));
            return true;
        }
        public override async Task<bool> EndGame(string message, bool isFailure = false)
        {
            _logger.LogInformation(message);
            base.gameTimer.Dispose();

            IsGameRunning = false;
            InternalGamePipe.Publish(new InternalSimpleGameUpdates(0, 0, true, ""));
            return true;
        }
        public override async Task<bool> WrongButtonPressed(ButtonPress bp, bool runNextCommand = false, bool updateScore = true, int amountToAdd = 1)
        {
            //There is no wrong button scoring in the Bonus round. You just go fase;
            //InternalGamePipe.Publish(new InternalSimpleGameUpdates(0, 1, false));
            base.PlayNextWrongSound();
            //PlaySound(LostBonusRound);
            //await EndGame("Wrong button pressed. Ending the bonus round");
            return false;
        }
        public override async Task<bool> NextCommand()
        {
            bonusTimer.Stop();
            if (IsGameRunning)
            {
                if (BapBasicGameHelper.ShouldWePerformARandomAction(6))
                {
                    lastNodeId = BapBasicGameHelper.GetRandomNodeId(buttons, lastNodeId, 2);
                    CrownNodeId = lastNodeId;
                    //Todo - A Timeout was lost
                    MsgSender.SendImage(lastNodeId, new(CrownSprite));
                }
                bonusTimer.Start();
                return await base.NextCommand();
            }

            return true;
        }

        public override Task<bool> CommandSent()
        {
            if (lastNodeId == CrownNodeId)
            {
                CrownNodeId = "";
            }
            return Task.FromResult(true);
        }
        public async override Task OnButtonPressed(ButtonPressedMessage e)
        {
            //This use to use TimeSinceLightTurnedOff
            if (e.NodeId == CrownNodeId)
            {
                correctScore += 20;
                MsgSender.PlayAudio(HitTheCrown);
                CrownNodeId = "";
            }
            else
            {
                await base.OnButtonPressed(e);
            }

        }

        public override async Task<bool> Start(int secondsToRun)
        {

            MsgSender.PlayAudio(StartOfEntireGame);
            bonusTimer = new System.Timers.Timer(MsDelay);
            await base.Start(secondsToRun);
            bonusTimer.Elapsed += NextButtonevent;
            bonusTimer.AutoReset = false;
            bonusTimer.Enabled = true;
            return true;
        }

        async void buttonPressCompleted(object? sender, GameEventMessage e)
        {
            //if (correctButtonsPressed >= 20)
            //{
            //    await EndGame("Reached 20 correct presses");
            //}
        }



        public virtual async void NextButtonevent(Object source, System.Timers.ElapsedEventArgs e)
        {
            await NextCommand();
        }

        public override void Dispose()
        {
            bonusTimer?.Dispose();
            base.Dispose();
        }
    }
}
