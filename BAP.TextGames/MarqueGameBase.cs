using MessagePipe;
using Microsoft.Extensions.Logging;
using NLog.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BAP.Web.Games
{

    public class MarqueLine
    {
        public List<string> NodeIdsOrderedLeftToRight { get; set; } = new();
        public List<ulong[,]> Images { get; set; } = new();
        public bool LeftToRight { get; set; } = true;
    }

    public abstract class MarqueGameBase : IBapGame
    {

        ILogger _logger { get; set; }
        public bool IsGameRunning { get; internal set; }
        public List<MarqueLine> Lines { get; internal set; } = new List<MarqueLine>();
        internal IBapMessageSender MsgSender { get; set; } = default!;
        public int AnimationRate { get; set; } = 200;
        public int FrameSpacing { get; set; } = 2;
        AnimationController Animate { get; set; }
        public bool ScrollAllTextOffScreen { get; set; }
        IDisposable subscriptions = default!;
        ISubscriber<AnimationCompleteMessage> AnimationCompletePipe { get; set; }
        public MarqueGameBase(ILogger logger, IBapMessageSender messageSender, AnimationController animationController, ISubscriber<AnimationCompleteMessage> animationCompletePipe)
        {
            _logger = logger;
            MsgSender = messageSender;
            Animate = animationController;
            AnimationCompletePipe = animationCompletePipe;
            var bag = DisposableBag.CreateBuilder();
            AnimationCompletePipe.Subscribe(async (x) => await AnimationComplete(x)).AddTo(bag);
            subscriptions = bag.Build();
        }
        ///// <summary>
        ///// This returns which image in the list and where it is on the button. Multiple images may be on the button - this is the left most one. 
        ///// </summary>
        ///// <param name="nodeId">Node Pressed</param>
        ///// <param name="numberOfFramesShifted">Number of Frames Shifted from the Start of the Animation Sequence</param>
        ///// <returns>Where the item is in the list or -1 if the node is not apart of the array. Returns -2 if no image was currently showing</returns>
        //private (int imageIdinList, int columnShift) GetImageInfoOnPressedButton(string nodeId, int numberOfFramesShifted)
        //{
        //    //Todo: This needs to take into account which direction the images are flowing;
        //    MarqueLine? marqueLine = Lines.FirstOrDefault(t => t.NodeIdsOrderedLeftToRight.Contains(nodeId));
        //    if (marqueLine == null)
        //    {
        //        return (-1, -1);
        //    }
        //    int indexOfItem = marqueLine.NodeIdsOrderedLeftToRight.IndexOf(nodeId);
        //    int leftColumnOfButtonId = ((8 + FrameSpacing) * (indexOfItem)) - FrameSpacing;
        //    int totalAvailablePixels = ((8 + FrameSpacing) * (marqueLine.NodeIdsOrderedLeftToRight.Count)) - FrameSpacing;
        //    int leftColumnOfButtonFromRight = totalAvailablePixels - leftColumnOfButtonId;
        //    if (leftColumnOfButtonFromRight < numberOfFramesShifted)
        //    {
        //        return (-2, -2);
        //    }
        //    int locationInBigMatrixShowingAtTheLeftOfTheButton = leftColumnOfButtonFromRight - numberOfFramesShifted;
        //    if (locationInBigMatrixShowingAtTheLeftOfTheButton < FrameSpacing)
        //    {
        //        return (-2, -2);
        //    }
        //    int imageIdInList = (locationInBigMatrixShowingAtTheLeftOfTheButton - FrameSpacing) / (8 + FrameSpacing);
        //    int columnShift = (locationInBigMatrixShowingAtTheLeftOfTheButton - FrameSpacing) % (8 + FrameSpacing);
        //    return (imageIdInList, columnShift);
        //}


        public abstract Task<bool> AnimationComplete(AnimationCompleteMessage animationCompleteMessage);

        public virtual async Task<bool> Start()
        {

            _logger.LogInformation($"Starting Marquee");

            IsGameRunning = true;
            //MsgSender.SendGeneralCommand(sbc);
            Animate.FrameRateInMillis = AnimationRate;
            List<BapAnimation> animations = new List<BapAnimation>();
            foreach (var line in Lines)
            {
                animations.AddRange(GenerateFramesForAllButtons(line));
            }
            //AnimationTickCountAtStartOfAnimation = Animate.CurrentFrameTickCount;
            Animate.AddOrUpdateAnimations(animations);

            return true;
        }

        private List<BapAnimation> GenerateFramesForAllButtons(MarqueLine line)
        {

            List<BapAnimation> animations = new List<BapAnimation>();
            ulong[,] bigMatrix = AnimationHelper.BuildBigMatrix(line.Images, line.NodeIdsOrderedLeftToRight.Count, FrameSpacing, ScrollAllTextOffScreen);
            foreach (var nodeId in line.NodeIdsOrderedLeftToRight)
            {
                animations.Add(new BapAnimation(new List<Frame>(), nodeId));
            }
            int currentLeftPixel = 0;
            int currentFrameId = 0;
            int screenWidth = line.NodeIdsOrderedLeftToRight.Count * 8;
            while (currentLeftPixel + screenWidth < bigMatrix.GetLength(1))
            {
                for (int i = 0; i < line.NodeIdsOrderedLeftToRight.Count; i++)
                {
                    animations[i].Frames.Add(new Frame(bigMatrix.ExtractMatrix(0, currentLeftPixel + (i * 8)), currentFrameId));
                }

                currentLeftPixel++;
                currentFrameId++;
            }
            return animations;

        }



        public virtual bool End(string reason)
        {
            MsgSender.SendUpdate(reason, true);
            Animate.Stop();
            IsGameRunning = false;
            return true;
        }

        public virtual async Task<bool> ForceEndGame()
        {
            End("Game was force Closed");
            return true;
        }

        public virtual void Dispose()
        {
            subscriptions.Dispose();
        }
    }

}
