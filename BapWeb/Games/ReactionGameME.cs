using MessagePipe;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BapWeb.Games;
using BapWeb.Pages;
using BapButton;
using BapShared;
using static BapButton.TinkerButtonGameMethods;

namespace BapWeb
{
    public class ReactionGameMeDescription : IGameDescription
    {
        public Type TypeOfInitialDisplayComponent => typeof(ReactionME);
        public string Name => "Mads and Ethan Reaction Game";
        public string Description => "Mads and Ethan's version of a reaction game";
        public string UniqueId => "bdf32580-a2e7-4b30-9e4b-42aa51f58a2c";

        public string ScoringModelVersion => "1.0.0";

    }
    public class ReactionGameME : ReactionGameBase
    {
        string lastFaceNodeId = "";
        bool lastFaceWasAFrownyFace = false;
        private const string FrownyFaceSound = "madsfrowny.wav";
        private const string TooManyWrong = "madstoomany.wav";
        internal override ILogger _logger { get; set; }

        public ReactionGameME(ILogger<ReactionGameBase> logger, ISubscriber<ButtonPressedMessage> buttonPressed, MessageSender messageSender) : base(buttonPressed, messageSender)
        {
            base.Initialize(minButtons: 2, useIfItWasLitForScoring: false);
            _logger = logger;
        }


        public override async Task<bool> Start(int secondsToRun)
        {
            string path = Path.Combine(".", "wwwroot", "sprites", "Emoji.png");
            SpriteParser spriteParser = new SpriteParser(path);
            var sprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 2, 9);
            MsgSender.SendCustomImage(new CustomImage() { ImageData = sprite, ImageId = 15 });
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
                    MsgSender.SendCommand(faceNodeId, new StandardButtonCommand(new ButtonDisplay(0, 0, 0, Patterns.NoPattern, 15, 2000)));

                }
                else
                {
                    lastFaceWasAFrownyFace = false;
                    MsgSender.SendCommand(faceNodeId, new StandardButtonCommand(new ButtonDisplay(0, 255, 0, Patterns.PlainSmilyFace, 0, 2000)));
                }
            }

            return true;

        }


        public async override Task OnButtonPressed(ButtonPressedMessage e)
        {
            if (lastFaceNodeId == e.NodeId && e.ButtonPress.TimeSinceLightTurnedOff < 150)
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

        public override ButtonDisplay GenerateNextButton()
        {
            return new ButtonDisplay(GetRandomInt(0, 255), GetRandomInt(0, 255), GetRandomInt(0, 255));
        }
    }

}
