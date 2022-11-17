using MessagePipe;
using Microsoft.Extensions.Logging;
using NLog.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BAP.TestUtilities
{
    public class LetterTestDescription : IBapGameDescription
    {
        public Type TypeOfInitialDisplayComponent => typeof(LetterTest);
        public string Name => "Letter Test";
        public string Description => "Test out if all the patterns are working.";

        public string UniqueId => "32c358da-c92f-40bd-b147-d6477799d582";
        public string ScoringModelVersion => "1.0.0";
    }

    public class LetterTest : IBapGame
    {
        ILogger _logger { get; set; }
        public bool IsGameRunning { get; internal set; }
        public Patterns StartOfRange { get; set; }
        public Patterns EndOfRange { get; set; } = Patterns.RightSingleQuoteMark;
        IBapMessageSender MsgSender { get; set; } = default!;
        public int AnimationRate { get; set; } = 200;
        public int LetterSpacing { get; set; } = 4;
        public bool Marque { get; set; } = false;
        public List<StandardTransitions> TransitionsToUse = new List<StandardTransitions>();
        AnimationController Animate { get; set; }
        StandardTransitions CurrentTransition { get; set; } = 0;
        private Patterns CurrentPattern { get; set; }
        private Patterns LeftMostPattern { get; set; }
        IDisposable subscriptions = default!;
        ISubscriber<AnimationCompleteMessage> AnimationCompletePipe { get; set; }
        public LetterTest(ILogger<LetterTest> logger, IBapMessageSender messageSender, AnimationController animationController, ISubscriber<AnimationCompleteMessage> animationCompletePipe)
        {
            _logger = logger;
            MsgSender = messageSender;
            Animate = animationController;
            AnimationCompletePipe = animationCompletePipe;
            var bag = DisposableBag.CreateBuilder();
            AnimationCompletePipe.Subscribe((x) => SendNextCommand(x)).AddTo(bag);
            subscriptions = bag.Build();
        }

        public async Task<bool> Start()
        {
            CurrentPattern = StartOfRange;
            LeftMostPattern = CurrentPattern;
            CurrentTransition = 0;

            _logger.LogInformation($"Starting patterns");

            IsGameRunning = true;
            MsgSender.SendUpdate("Starting Letter Test", false);
            //ButtonDisplay msg = new ButtonDisplay(128, 64, 32, Patterns.NoPattern, 0, turnOffAfterMillis: 3000);
            ////
            //MsgSender.SendCommand(MsgSender.GetConnectedButtons().First(), new StandardButtonCommand(msg));
            MsgSender.SendUpdate($"Sending custom image to all Buttons", false);
            string path = Path.Combine(".", "wwwroot", "sprites", "Emoji.png");
            SpriteParser spriteParser = new SpriteParser(path);
            var sword = spriteParser.GetSpriteAsMatrix(4, 5, 24, 20, 16, 5, 7);

            AnimationHelper.MergeMatrices(sword, AnimationHelper.GetMatrix(Patterns.Border, StandardColorPalettes.Default[3]));
            MsgSender.SendUpdate("Displaying custom image on button", false);
            MsgSender.SendImageToAllButtons(new ButtonImage(sword));
            //MsgSender.SendGeneralCommand(sbc);

            await Task.Delay(2000);
            Animate.FrameRateInMillis = AnimationRate;
            SendNextCommand(advanceForward: false);
            return true;
        }

        private void SendNextCommand(AnimationCompleteMessage? animationCompleteMessage = null, bool advanceForward = true)
        {
            //Todo - This is really just for testing to see if all of the nodes are completing the animation at the same time.
            if (animationCompleteMessage != null)
            {
                _logger.LogDebug($"There are {MsgSender.ButtonCount} buttons connected and {animationCompleteMessage.NodeIds.Count} completed the anmation");

            }

            if (CurrentPattern > EndOfRange)
            {
                End("Reached the last pattern");
            }
            else
            {
                if (advanceForward)
                {
                    CurrentPattern++;
                }

                if (Marque)
                {
                    if (animationCompleteMessage == null)
                    {
                        MsgSender.ClearButtons();
                    }
                    List<string> nodes = MsgSender.GetConnectedButtonsInOrder().ToList();
                    int numberOfPatternsDisplayed = (int)CurrentPattern - (int)StartOfRange + 1;
                    int numberOfItemsToShow = Math.Min(numberOfPatternsDisplayed, MsgSender.ButtonCount);
                    if (numberOfItemsToShow == nodes.Count)
                    {
                        LeftMostPattern = (Patterns)((int)LeftMostPattern + 1);
                    }
                    List<BapAnimation> animations = new List<BapAnimation>();

                    ulong[,] bigMatrix = new ulong[1, 1];
                    int numberOfPatternsShowing = 0;
                    for (int i = 0; i <= nodes.Count; i++)
                    {
                        Patterns patternToDisplay = (Patterns)((int)LeftMostPattern + numberOfPatternsShowing);
                        BapColor bapColor = new BapColor(0, 255, 0);
                        if (nodes.Count - i > numberOfItemsToShow)
                        {
                            patternToDisplay = Patterns.AllOneColor;
                            bapColor = new BapColor(0, 0, 0);
                        }
                        else
                        {
                            numberOfPatternsShowing++;
                        }
                        if (i == 0)
                        {
                            bigMatrix = (AnimationHelper.GetMatrix(patternToDisplay, bapColor));
                        }
                        else
                        {

                            //Todo: his does not handle the end of the patterns correctly. If we are at the end of the array we just need to append blankness.
                            bigMatrix = bigMatrix.ConcatOnRow(new ulong[8, LetterSpacing]);
                            bigMatrix = bigMatrix.ConcatOnRow(AnimationHelper.GetMatrix(patternToDisplay, bapColor));
                        }

                    }
                    for (int buttonNumber = 0; buttonNumber < nodes.Count; buttonNumber++)
                    {

                        List<Frame> frames = new();
                        for (int s = 0; s < 8 + LetterSpacing; s++)
                        {
                            int startingPoint = buttonNumber * 8 + s;
                            frames.Add(new Frame(bigMatrix.ExtractMatrix(0, startingPoint), s));
                        }

                        BapAnimation animation = new(frames, nodes[buttonNumber]);
                        animations.Add(animation);
                    }
                    Animate.AddOrUpdateAnimations(animations);
                }
                else
                {

                    _logger.LogInformation($"Writing {CurrentPattern}");
                    MsgSender.SendUpdate($"Writing {CurrentPattern}", false);
                    int indexOfCurrentTransition = TransitionsToUse.IndexOf(CurrentTransition);
                    if (indexOfCurrentTransition == TransitionsToUse.Count() - 1)
                    {
                        CurrentTransition = TransitionsToUse.First();
                    }
                    else
                    {
                        CurrentTransition = TransitionsToUse[indexOfCurrentTransition + 1];
                    }
                    CurrentTransition = (int)CurrentTransition < 8 ? CurrentTransition + 1 : 0;
                    List<Frame> frames = AnimationHelper.GetFrames(AnimationHelper.GetMatrix(CurrentPattern, new BapColor(0, 255, 0)), CurrentTransition);


                    Animate.AddOrUpdateAnimation(new BapAnimation(frames, loopCount: CurrentTransition == StandardTransitions.NoTransition ? 16 : 0));

                }

            }
        }

        public bool End(string reason)
        {
            MsgSender.SendUpdate(reason, true);
            Animate.Stop();
            IsGameRunning = false;
            return true;
        }

        public async Task<bool> ForceEndGame()
        {
            End("Game was force Closed");
            return true;
        }

        public void Dispose()
        {
            subscriptions.Dispose();
        }
    }

}
