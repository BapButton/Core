﻿using System;
using System.Threading.Tasks;
using MessagePipe;
using Microsoft.AspNetCore.Components;

namespace BAP.TestUtilities.Components
{
    [MenuItem("Pattern Testing", "Send patterns in order to button", false, "ea0d8577-609f-40e4-b05e-f02004b080a4")]
    public partial class PatternTesting : ComponentBase, IDisposable
    {
        [Inject]
        IGameProvider gameHandler { get; set; } = default!;
        [Inject]
        ISubscriber<GameEventMessage> gameEventMessages { get; set; } = default!;

        IDisposable subscription = default!;
        private LetterTest game = default!;
        private int transitionOrTransitionGroups = -1;
        private string LastMessage = "";
        private Patterns StartOfRange { get; set; } = Patterns.NoPattern;
        private Patterns EndOfRange { get; set; } = Patterns.RightSingleQuoteMark;
        private int AnimationSpeed { get; set; } = 200;

        protected override void OnInitialized()
        {

            base.OnInitialized();
            gameHandler.UpdateToNewGameType(typeof(LetterTest));
            subscription = gameEventMessages.Subscribe(async (x) => await GameUpdate(x));
            gameHandler.UpdateToNewGameType(typeof(LetterTest));
            game = (LetterTest)gameHandler.CurrentGame!;

        }

        async Task GameUpdate(GameEventMessage e)
        {
            LastMessage = e.Message;
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }


        async Task<bool> StartGame()
        {
            game.StartOfRange = StartOfRange;
            game.EndOfRange = EndOfRange;
            game.AnimationRate = AnimationSpeed;

            bool Marque = false;
            List<StandardTransitions> transitionsToUse = new List<StandardTransitions>();
            if (transitionOrTransitionGroups == -2)
            {
                Marque = true;
            }
            else if (transitionOrTransitionGroups == -1)
            {
                //Todo - This needs to actually add all of the standard transitions

                for (int i = 0; i <= (int)StandardTransitions.SlideOnScreenFromBottom; i++)
                {
                    transitionsToUse.Add((StandardTransitions)i);
                }
            }
            else
            {
                transitionsToUse.Add((StandardTransitions)transitionOrTransitionGroups);
            }
            game.Marque = Marque;
            game.TransitionsToUse = transitionsToUse;
            await game.Start();
            return true;
        }
        async Task<bool> EndGame()
        {
            game.End("Game Ended by User");
            return await Task.FromResult(true);
        }
        public void Dispose()
        {
            // unsubscribe event when browser is closed.
            subscription?.Dispose();
        }
    }
}
