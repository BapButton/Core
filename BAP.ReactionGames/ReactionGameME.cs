using MessagePipe;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BAP.Types;
using BAP.Helpers;
using static BAP.Helpers.BapBasicGameHelper;
using BAP.ReactionGames.Components;

namespace BAP.ReactionGames
{

    public class ReactionGameME : ReactionGameBase
    {
        string lastFaceNodeId = "";
        bool lastFaceWasAFrownyFace = false;
        private const string FrownyFaceSound = "madsfrowny.wav";
        private const string TooManyWrong = "madstoomany.wav";
        internal override ILogger _logger { get; set; }
        public ulong[] customImage { get; set; } = new ulong[64];

        public ReactionGameME(ILogger<ReactionGameBase> logger, ISubscriber<ButtonPressedMessage> buttonPressed, IBapMessageSender messageSender) : base(buttonPressed, messageSender)
        {
            base.Initialize(minButtons: 2, useIfItWasLitForScoring: false);
            _logger = logger;
        }


        public override async Task<bool> Start(int secondsToRun)
        {
            string path = Path.Combine(".", "wwwroot", "sprites", "Emoji.png");
            SpriteParser spriteParser = new SpriteParser(path);
            customImage = spriteParser.GetSprite(4, 5, 24, 20, 16, 2, 9);
            await Task.Delay(100);
            await base.Start(secondsToRun);
            lastFaceNodeId = "";
            return true;
        }


        public async override Task<bool> NextCommand()
        {
            await base.NextCommand();
            bool sendFace = ShouldWePerformARandomAction(3);
            if (lastFaceNodeId == lastNodeId)
            {
                lastFaceNodeId = "";
            }
            if (sendFace)
            {
                bool sendFrownyFace = ShouldWePerformARandomAction(3);
                string faceNodeId = GetRandomNodeId(buttons, lastNodeId, 0);
                lastFaceNodeId = faceNodeId;
                if (sendFrownyFace)
                {
                    lastFaceWasAFrownyFace = true;
                    MsgSender.SendImage(faceNodeId, new ButtonImage(customImage));

                }
                else
                {
                    lastFaceWasAFrownyFace = false;
                    MsgSender.SendImage(faceNodeId, new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.PlainSmilyFace), new BapColor(0, 255, 0)));
                }
            }

            return true;

        }


        public async override Task OnButtonPressed(ButtonPressedMessage e)
        {
            //This use to use TimeSinceLightTurnedOff
            if (lastFaceNodeId == e.NodeId)
            {
                if (lastFaceWasAFrownyFace)
                {
                    await EndGame("It was a frowny Face", true);
                    MsgSender.PlayAudio(FrownyFaceSound);
                }
                else
                {
                    //this increments the score by 3 total because rightbuttonpressed does one.;
                    correctScore++;
                    correctScore++;
                    await RightButtonPressed(e.ButtonPress, false);
                }
            }
            else
            {
                await base.OnButtonPressed(e);
            }

        }


        public override async Task<bool> WrongButtonPressed(ButtonPress bp, bool runNextCommand = false, bool updateScore = true, int amountToAdd = 1)
        {
            await base.WrongButtonPressed(bp);
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
