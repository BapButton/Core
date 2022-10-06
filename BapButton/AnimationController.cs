using MessagePipe;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BapShared;

namespace BapButton
{

    internal class FrameHolder
    {
        public int CurrentLoopInitialFrameTickCount { get; set; }
        public int LoopToFrameId { get; set; }
        public int LoopCount { get; set; }
        public int CurrentLoopNumber { get; set; }
        public List<Frame> Frames { get; set; } = new();
    }

    public class AnimationController : IDisposable
    {
        ILogger<AnimationController> Logger { get; set; }
        MessageSender MsgSender { get; set; }
        CancellationTokenSource timerUpdator = new();
        PeriodicTimer? timer = null;
        public int FrameRateInMillis = 50;
        public bool AnnounceEveryFrameMovement { get; set; }
        private ConcurrentDictionary<string, FrameHolder> ButtonFrames { get; set; } = new();
        public int CurrentFrameTickCount { get; private set; }
        IPublisher<AnimationCompleteMessage> AnimationCompleteSender { get; set; }
        public AnimationController(ILogger<AnimationController> logger, MessageSender msgSender, IPublisher<AnimationCompleteMessage> animationCompleteSender)
        {
            Logger = logger;
            MsgSender = msgSender;
            AnimationCompleteSender = animationCompleteSender;

        }


        public void Stop(bool clearScreen = true)
        {

            timerUpdator.Cancel();
            Logger.LogDebug("Stopping Animation Controller");
            ButtonFrames = new();
            if (clearScreen)
            {
                MsgSender.ClearButtons();
            }

        }
        public void AddOrUpdateAnimations(List<BapAnimation> animations)
        {
            foreach (var animation in animations)
            {
                FrameHolder frameHolder = new()
                {
                    Frames = animation.Frames,
                    LoopCount = animation.LoopCount,
                    LoopToFrameId = animation.LoopToFrameId
                };
                ButtonFrames[animation.NodeId] = frameHolder;
            }

            StartAnimationClock();
        }

        public void AddOrUpdateAnimation(BapAnimation animation)
        {
            List<BapAnimation> animations = new List<BapAnimation>();
            if (string.IsNullOrEmpty(animation.NodeId))
            {
                foreach (var nodeId in MsgSender.GetConnectedButtons())
                {
                    animations.Add(new BapAnimation(animation.Frames, nodeId, animation.LoopToFrameId, animation.LoopCount));
                }
            }
            else
            {
                animations.Add(animation);
            }
            AddOrUpdateAnimations(animations);
        }

        private void StopAllAnimationsIfNoNodes()
        {
            if (ButtonFrames.IsEmpty)
            {
                timerUpdator.Cancel();
            }
        }
        public void StopAnimationForNode(string nodeId, bool leaveImageInPlaceOnButton)
        {
            ButtonFrames.Remove(nodeId, out _);
            if (!leaveImageInPlaceOnButton)
            {
                MsgSender.SendInternalCustomImage(nodeId, new InternalCustomImage());
            }
            StopAllAnimationsIfNoNodes();
        }

        private void RunAllAnimations()
        {
            //Id do this just in use the dictionary changes while looping. As this is running on one thread for the most part it seems unlikely
            ////but its seems like button presses and things could definitly introduce more threads to the picture. 

            if (!ButtonFrames.IsEmpty)
            {
                List<KeyValuePair<string, FrameHolder>>? CurrentAnimations = ButtonFrames.ToList();
                int tickCountAtStartOfRunAllAnimations = CurrentFrameTickCount;
                List<string> animationCompleteNodeIds = new();
                foreach (var nodeAndFrameHolder in CurrentAnimations)
                {
                    if (nodeAndFrameHolder.Value.CurrentLoopInitialFrameTickCount == 0)
                    {
                        nodeAndFrameHolder.Value.CurrentLoopInitialFrameTickCount = tickCountAtStartOfRunAllAnimations;
                        if (nodeAndFrameHolder.Value.LoopToFrameId > 0 && nodeAndFrameHolder.Value.CurrentLoopNumber > 0)
                        {
                            nodeAndFrameHolder.Value.CurrentLoopInitialFrameTickCount = tickCountAtStartOfRunAllAnimations - nodeAndFrameHolder.Value.LoopToFrameId;
                        }
                    }
                    var frameHolder = nodeAndFrameHolder.Value;
                    int currentFrameId = tickCountAtStartOfRunAllAnimations - frameHolder.CurrentLoopInitialFrameTickCount;

                    Frame? frameToSend = frameHolder.Frames.FirstOrDefault(t => t.FrameNumberToShow == currentFrameId);
                    int frameIdMax = frameHolder.Frames.Max(t => t.FrameNumberToShow);
                    bool showedLastFrame = false;
                    if (frameToSend != null)
                    {
                        MsgSender.SendInternalCustomImage(nodeAndFrameHolder.Key, new InternalCustomImage(frameToSend.Image));

                        if (frameToSend.FrameNumberToShow >= frameIdMax)
                        {
                            showedLastFrame = true;

                        }
                    }
                    if (currentFrameId > frameIdMax)
                    {
                        Logger.LogDebug("The final frame must have been missed or frame counting misfired. Probably a bug");
                        showedLastFrame = true;
                    }
                    if (showedLastFrame)
                    {
                        if (frameHolder.LoopCount < frameHolder.CurrentLoopNumber + 1)
                        {
                            animationCompleteNodeIds.Add(nodeAndFrameHolder.Key);
                        }
                        else
                        {
                            frameHolder.CurrentLoopNumber++;
                            frameHolder.CurrentLoopInitialFrameTickCount = 0;
                        }


                    }
                }
                if (animationCompleteNodeIds.Any())
                {
                    AnimationCompleteSender.Publish(new AnimationCompleteMessage(animationCompleteNodeIds));
                }
            }

        }


        /// <summary>
        ///If you await this you will move on when the timer is cancelled. So probably you don't want to await it. 
        //It is safe to call this even if the animation clock has already been started. It won't do anything. 
        /// </summary>
        /// <returns></returns>
        private async Task StartAnimationClock()
        {
            bool startingNewAnimationRun = false;
            if (timerUpdator.IsCancellationRequested)
            {
                timerUpdator = new();
                startingNewAnimationRun = true;

            }
            else if (timer == null)
            {
                startingNewAnimationRun = true;
            }
            if (startingNewAnimationRun)
            {
                timer = new PeriodicTimer(TimeSpan.FromMilliseconds(FrameRateInMillis));
                var timerToken = timerUpdator.Token;

                RunAllAnimations();
                while (await timer.WaitForNextTickAsync(timerToken))
                {
                    RunAllAnimations();
                    CurrentFrameTickCount++;
                };
            }


        }

        public void Dispose()
        {
            if (timerUpdator != null)
            {
                timerUpdator.Cancel();
                timerUpdator.Dispose();
            }
        }
    }
}
