﻿using MessagePipe;
using Microsoft.Extensions.Logging;
using NLog.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BapButton;
using BapShared;
using static BapButton.TinkerButtonGameMethods;

namespace BapWeb.Games
{
    public abstract class ReactionGameBase : ITinkerGame, IDisposable
    {
        internal abstract ILogger _logger { get; set; }
        public bool IsGameRunning { get; internal set; }
        internal string lastNodeId = "";
        internal int correctScore = 0;
        internal int wrongScore = 0;
        internal List<string> buttons { get; set; } = new List<string>();
        public bool GamePaused { get; internal set; }
        ISubscriber<ButtonPressedMessage> ButtonPressedPipe { get; set; } = default!;
        IDisposable subscriptions = default!;
        internal PausableTimer gameTimer;
        List<string> _correctSounds = new List<string>();
        List<string> _wrongSounds = new List<string>();
        private int _minButtons;
        private bool _useIfItWasLitForScoring;
        internal MessageSender MsgSender { get; set; } = default!;

        public virtual async Task<bool> Start()
        {
            return await Start(120);
        }

        public async Task<bool> ForceEndGame()
        {
            return await EndGame("Game was force Closed");
        }

        public ReactionGameStatus GetStatus()
        {
            return new ReactionGameStatus()
            {
                CorrectScore = correctScore,
                WrongScore = wrongScore,
                TimeRemaining = gameTimer?.TimeRemaining() ?? new TimeSpan()
            };

        }

        internal ReactionGameBase(ISubscriber<ButtonPressedMessage> buttonPressed, MessageSender messageSender)

        {
            ButtonPressedPipe = buttonPressed;
            MsgSender = messageSender;
            var bag = DisposableBag.CreateBuilder();
            ButtonPressedPipe.Subscribe(async (x) => await ButtonPressed(x)).AddTo(bag);
            subscriptions = bag.Build();
            gameTimer = new();
        }

        public abstract ButtonDisplay GenerateNextButton();

        public async virtual Task<bool> Start(int secondsToRun)
        {
            if (IsGameRunning)
            {
                return false;
            }

            IsGameRunning = true;
            MsgSender.StopAllAudio();
            MsgSender.SendUpdate("Game Started");
            if (secondsToRun > 0)
            {
                gameTimer = new PausableTimer(secondsToRun * 1000);
                gameTimer.Start();
                gameTimer.Elapsed += EndGameEvent;
                gameTimer.AutoReset = false;
                gameTimer.Enabled = true;
            }

            buttons = MsgSender.GetConnectedButtons();
            correctScore = 0;
            wrongScore = 0;
            int buttonCount = buttons.Count;
            if (buttonCount < _minButtons)
            {
                MsgSender.SendUpdate("Not Enough Buttons", fatalError: true);
                await EndGame($"Not enough Buttons. The game requires {_minButtons} buttons but there is only {buttonCount} connected");
                return false;
            }
            MsgSender.SendGeneralCommand(new StandardButtonCommand(new ButtonDisplay(0, 0, 0)));
            await NextCommand();

            return true;
        }


        public virtual async void EndGameEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            await EndGame("Time Expired - Game Ended");
        }
        public virtual async Task<bool> EndGame(string message, bool isFailure = false)
        {
            _logger.LogInformation(message);
            gameTimer?.Dispose();
            //ToDo This is terrible. WrongScore should not be embedded in reaction game base
            isFailure = isFailure | wrongScore > 5;
            foreach (var button in MsgSender.GetConnectedButtons())
            {
                ushort red = isFailure ? (ushort)255 : (ushort)0;
                ushort blue = isFailure ? (ushort)0 : (ushort)255;
                ButtonDisplay standardButtonMessage = new ButtonDisplay(red, (ushort)0, blue, turnOffAfterMillis: 3000);
                MsgSender.SendCommand(button, new StandardButtonCommand(standardButtonMessage));
            }

            IsGameRunning = false;
            MsgSender.SendUpdate("Game Ended", true);
            return true;
        }

        public async virtual Task<bool> NextCommand()
        {
            string nextNodeId = TinkerButtonGameMethods.GetRandomNodeId(buttons, lastNodeId, 4);
            lastNodeId = nextNodeId;
            MsgSender.SendCommand(nextNodeId, new StandardButtonCommand(GenerateNextButton()));
            await CommandSent();
            return true;

        }
        public async virtual Task<bool> CommandSent()
        {
            return await Task.FromResult(true);

        }
        public virtual string GetNextSound(List<string> sounds)
        {
            return TinkerButtonGameMethods.GetRandomNodeId(sounds);
        }

        public virtual void PlayNextWrongSound()
        {
            MsgSender.PlayAudio(GetNextSound(_wrongSounds));
        }
        public virtual void PlayNextRightSound()
        {
            MsgSender.PlayAudio(GetNextSound(_correctSounds));
        }
        public virtual Task<bool> WrongButtonPressed(ButtonPress bp, bool runNextCommand = false, bool updateScore = true, int amountToAdd = 1)
        {
            wrongScore++;
            PlayNextWrongSound();
            if (_useIfItWasLitForScoring)
            {
                _logger.LogInformation($"Wrong answer - the light had been off for {bp.TimeSinceLightTurnedOff} milliseconds");
            }
            return Task.FromResult(true);
        }

        public virtual async Task<bool> RightButtonPressed(ButtonPress bp, bool runNextCommand = true, bool updateScore = true, int amountToAdd = 1)
        {
            if (updateScore)
            {
                correctScore += amountToAdd;
            }
            PlayNextRightSound();

            if (runNextCommand)
            {
                await NextCommand();
            }
            MsgSender.SendUpdate("Score Updated");
            return true;
        }
        private async Task ButtonPressed(ButtonPressedMessage e)
        {
            await OnButtonPressed(e);
        }

        public async virtual void PauseGame(bool pauseTimerAlso = false)
        {
            GamePaused = true;
            if (pauseTimerAlso)
            {
                gameTimer.Pause();
            }
        }
        public async virtual void UnPauseGame()
        {
            GamePaused = false;
            if (!gameTimer.Enabled)
            {
                gameTimer.Resume();
            }
        }

        public async virtual Task OnButtonPressed(ButtonPressedMessage e)
        {
            if (IsGameRunning && !GamePaused)
            {
                if (lastNodeId == e.NodeId || _useIfItWasLitForScoring && e.ButtonPress.TimeSinceLightTurnedOff < 150)
                {
                    await RightButtonPressed(e.ButtonPress);
                }
                else
                {
                    await WrongButtonPressed(e.ButtonPress);

                }
                MsgSender.SendUpdate("Button Pressed");
            }
        }

        internal virtual bool Initialize(List<string>? correctSounds = null, List<string>? wrongSounds = null, int minButtons = 2, bool useIfItWasLitForScoring = false)
        {
            _minButtons = minButtons;
            _useIfItWasLitForScoring = useIfItWasLitForScoring;
            if (correctSounds == null)
            {
                _correctSounds.Add("open_001.wav");
            }
            else
            {
                _correctSounds = correctSounds ?? new List<string>();
            }
            if (wrongSounds == null)
            {
                _wrongSounds.Add("error_004.wav");
            }
            else
            {
                _wrongSounds = wrongSounds ?? new List<string>();
            }
            return true;
        }

        public virtual void Dispose()
        {
            if (subscriptions != null)
            {
                subscriptions.Dispose();
            }
        }
    }
    public class ReactionGameStatus
    {
        public int CorrectScore { get; set; }
        public int WrongScore { get; set; }
        public TimeSpan TimeRemaining { get; set; }
    }
}
