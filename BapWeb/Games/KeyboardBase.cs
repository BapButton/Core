using MessagePipe;
using Microsoft.Extensions.Logging;
using NLog.Common;
using SixLabors.ImageSharp.ColorSpaces.Companding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BapButton;
using BapShared;

namespace BapWeb.Games
{
    public abstract class KeyboardBase : IKeyboardController
    {
        public ILogger _logger { get; set; }
        private List<string> NodeIdsToUse { get; set; } = new();

        public Dictionary<string, char> KeyboardValues = new();
        public MessageSender MsgSender { get; internal set; }
        private IPublisher<KeyboardKeyPressedMessage> KeyboardKeyPressedSender { get; set; } = default!;
        private BapColor ColorForCharacters { get; set; } = default!;
        private ButtonDisplay? DefaultShowOnPress { get; set; } = default!;
        private ButtonDisplay? ShowOnCorrectPress { get; set; } = null;
        private int CurrentPlaceInArray { get; set; }
        private char CurrentCorrectCharId { get; set; }
        private char leftArrow = '←';
        private char rightArrow = '→';
        private bool moveBackwards = false;
        private bool AlwaysUseAllButtons = false;
        private bool PlayKeyPressSound = false;

        ISubscriber<ButtonPressedMessage> ButtonPressedPipe { get; set; } = default!;
        IDisposable subscriptions = default!;
        List<string> NodeIdsToAvoid = new();
        List<char> ValuesToDisplay = new();
        public int CurrentNumber { get; set; }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
        }
        private bool _isEnabled { get; set; }

        public bool IsConfigured
        {
            get
            {
                return _isConfigured;
            }
        }
        private bool _isConfigured { get; set; }

        public KeyboardBase(ILogger logger, ISubscriber<ButtonPressedMessage> buttonPressed, MessageSender msgSender, IPublisher<KeyboardKeyPressedMessage> keyboardKeyPressedSender)
        {
            _logger = logger;
            ButtonPressedPipe = buttonPressed;
            KeyboardKeyPressedSender = keyboardKeyPressedSender;
            MsgSender = msgSender;
            var bag = DisposableBag.CreateBuilder();
            ButtonPressedPipe.Subscribe(async (x) => await OnButtonPressed(x)).AddTo(bag);
            subscriptions = bag.Build();
        }

        public async Task OnButtonPressed(ButtonPressedMessage buttonPressed)
        {
            if (IsEnabled)
            {

                if (KeyboardValues.TryGetValue(buttonPressed.NodeId, out char value))
                {
                    if (PlayKeyPressSound)
                    {
                        MsgSender.PlayAudio("KeyTouch.mp3");
                    }

                    if (value == rightArrow || value == leftArrow)
                    {
                        MoveKeyboard(value);
                    }
                    else
                    {
                        Console.WriteLine($"Publishing {value}");
                        KeyboardKeyPressedSender.Publish(new KeyboardKeyPressedMessage(value, buttonPressed.ButtonPress));
                    }

                }
                else
                {
                    _logger.LogDebug($"{buttonPressed.NodeId} is not a part of the current keyboard");
                }
            }
        }
        public virtual void SetupKeyboard(List<char> valuesToDisplay, BapColor colorForCharacters, ButtonDisplay? defaultShowOnPress = null, List<string>? nodesToAvoid = null, bool alwaysUseAllButtons = false, bool playKeyPressSound = false)
        {
            NodeIdsToAvoid = nodesToAvoid ?? new List<string>();
            ValuesToDisplay = valuesToDisplay;
            DefaultShowOnPress = defaultShowOnPress;
            ColorForCharacters = colorForCharacters;
            KeyboardValues = new Dictionary<string, char>();
            CurrentPlaceInArray = 0;
            _isConfigured = true;
            moveBackwards = false;
            CurrentCorrectCharId = default;
            AlwaysUseAllButtons = alwaysUseAllButtons;
            PlayKeyPressSound = playKeyPressSound;
            NodeIdsToUse = MsgSender.GetConnectedButtonsInOrder().Except(NodeIdsToAvoid).ToList();
            LayoutKeyboard();
        }

        private void LayoutKeyboard()
        {

            int currentButtonCount = NodeIdsToUse.Count;
            bool reachedEndOfArray = true;

            if (moveBackwards)
            {
                int currentlyDisplayedItems = KeyboardValues.Where(t => t.Value != leftArrow && t.Value != rightArrow).Count();
                var tempcpa = CurrentPlaceInArray - currentlyDisplayedItems;
                if (tempcpa > 0)
                {
                    tempcpa -= currentButtonCount - 2;
                }
                else
                {
                    tempcpa -= currentButtonCount - 1;
                }
                CurrentPlaceInArray = tempcpa;
                //If it is 1 then we don't need the arrow
                CurrentPlaceInArray = CurrentPlaceInArray < 2 ? 0 : CurrentPlaceInArray;
            }
            KeyboardValues = new Dictionary<string, char>();
            int itemsToLayout = Math.Min(currentButtonCount, ValuesToDisplay.Count - CurrentPlaceInArray);
            //Possible items to display
            int possibleItemsToLayout = (ValuesToDisplay.Count - CurrentPlaceInArray);
            //Layout the Smaller number
            itemsToLayout = Math.Min(itemsToLayout, possibleItemsToLayout);

            int totalValuesDisplayByEndOfLoop = CurrentPlaceInArray + itemsToLayout;
            if (totalValuesDisplayByEndOfLoop != ValuesToDisplay.Count)
            {
                //Because this is not the beginning of the array then a spot is needed for the first arrow
                itemsToLayout = CurrentPlaceInArray > 0 ? itemsToLayout - 1 : itemsToLayout;
            }
            bool showLeftArrow = CurrentPlaceInArray > 0;
            if (AlwaysUseAllButtons && totalValuesDisplayByEndOfLoop == ValuesToDisplay.Count && CurrentPlaceInArray > 0)
            {
                //Need to move currentPlaceInArray and update itemsToLayout so that the most number of things is displayed. 
                itemsToLayout = currentButtonCount;
                CurrentPlaceInArray = ValuesToDisplay.Count - currentButtonCount + 1;
            }
            else if (showLeftArrow)
            {
                if (itemsToLayout < currentButtonCount)
                {
                    itemsToLayout++;
                }
                else
                {
                    totalValuesDisplayByEndOfLoop--;
                }

            }
            bool showRightArrow = totalValuesDisplayByEndOfLoop < ValuesToDisplay.Count;


            int adjustment = CurrentPlaceInArray;
            for (int i = 0; i < itemsToLayout; i++)
            {
                if (i == 0 && showLeftArrow)
                {
                    KeyboardValues.Add(NodeIdsToUse[i], leftArrow);
                    adjustment--;
                }
                else if (i == itemsToLayout - 1 && showRightArrow)
                {

                    KeyboardValues.Add(NodeIdsToUse[i], rightArrow);
                    reachedEndOfArray = false;
                }
                else
                {

                    KeyboardValues.Add(NodeIdsToUse[i], ValuesToDisplay[i + adjustment]);
                }
            }
            if (!reachedEndOfArray)
            {
                int numberToAdd = itemsToLayout;
                if (showLeftArrow) { numberToAdd--; }
                if (showRightArrow) { numberToAdd--; }
                CurrentPlaceInArray = CurrentPlaceInArray + numberToAdd;
            }
        }

        private string GetNodeIdByChar(char valueToLookup)
        {
            var currentNode = KeyboardValues.FirstOrDefault(t => t.Value == valueToLookup);
            return currentNode.Key ?? "";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="showOnCorrectPress">What do you want to use </param>
        /// <param name="correctNumber"></param>
        /// <param name="turnOnInMillis"></param>
        /// <returns>Return true if node is found and message sent. Otherwise send false.</returns>
        public bool UpdateCorrectValue(char correctChar, bool wasLastPressCorrect, ButtonDisplay? showOnCorrectPress = null)
        {

            ShowOnCorrectPress = showOnCorrectPress;

            if (correctChar == CurrentCorrectCharId)
            {
                return true;
            }
            else if (CurrentCorrectCharId != 0)
            {
                string oldNodeId = GetNodeIdByChar(CurrentCorrectCharId);
                if (!string.IsNullOrEmpty(oldNodeId))
                {
                    ButtonDisplay initial = new(ColorForCharacters, EnumHelper.GetEnumFromCharacter(CurrentCorrectCharId));
                    ButtonDisplay onPress = new(ColorForCharacters, EnumHelper.GetEnumFromCharacter(CurrentCorrectCharId));
                    //ButtonDisplay onPress = DefaultShowOnPress ?? initial;
                    ButtonDisplay timeOut = new(ColorForCharacters, EnumHelper.GetEnumFromCharacter(CurrentCorrectCharId));
                    if (onPress.TurnOffAfterMillis == 0)
                    {
                        onPress.TurnOffAfterMillis = 500;
                    }
                    if (wasLastPressCorrect && ShowOnCorrectPress != null)
                    {
                        initial = ShowOnCorrectPress;
                        initial.TurnOffAfterMillis = 500;

                    }
                    else if (wasLastPressCorrect == false)
                    {
                        //this is new for the no on press stuff and it is totally wrong. This needs to send to the node that was pressed wrong with the number of that button.
                        initial = DefaultShowOnPress ?? initial;
                        initial.TurnOffAfterMillis = 500;
                        MsgSender.SendCommand(oldNodeId, new StandardButtonCommand(initial, onPress, timeOut));
                    }

                    MsgSender.SendCommand(oldNodeId, new StandardButtonCommand(initial, onPress, timeOut));
                }
            }

            CurrentCorrectCharId = correctChar;
            string nodeId = GetNodeIdByChar(correctChar);
            if (!string.IsNullOrEmpty(nodeId))
            {
                ButtonDisplay initial = new(ColorForCharacters, EnumHelper.GetEnumFromCharacter(correctChar));
                ButtonDisplay onPress = initial;
                //ButtonDisplay onPress = showOnCorrectPress ?? DefaultShowOnPress ?? initial;
                if (onPress.TurnOffAfterMillis == 0)
                {
                    onPress.TurnOffAfterMillis = 500;
                }
                MsgSender.SendCommand(nodeId, new StandardButtonCommand(initial, onPress, initial));
                return true;
            }

            return false;
        }
        private void MoveKeyboard(char arrow)
        {
            if (arrow == leftArrow)
            {
                moveBackwards = true;
            }
            else
            {
                moveBackwards = false;
            }
            LayoutKeyboard();
            ShowKeyboard();
        }

        public virtual void ShowKeyboard(int turnOnInMillis = 0)
        {
            Enable();
            //I should really check if all of the buttons are in use and then use general command;
            foreach (var keyAndNodeId in KeyboardValues)
            {
                bool isArrow = keyAndNodeId.Value == rightArrow || keyAndNodeId.Value == leftArrow;
                ButtonDisplay initial = new(ColorForCharacters, EnumHelper.GetEnumFromCharacter(keyAndNodeId.Value));
                ButtonDisplay onPress = new(ColorForCharacters, EnumHelper.GetEnumFromCharacter(keyAndNodeId.Value));
                //ButtonDisplay onPress = isArrow ? initial : DefaultShowOnPress ?? initial;
                //if (CurrentCorrectCharId == keyAndNodeId.Value)
                //{
                //    onPress = ShowOnCorrectPress ?? DefaultShowOnPress ?? initial;
                //}
                //if (onPress.TurnOffAfterMillis == 0)
                //{
                //    onPress.TurnOffAfterMillis = 500;
                //}
                MsgSender.SendCommand(keyAndNodeId.Key, new StandardButtonCommand(initial, onPress, initial, turnOnInMillis: turnOnInMillis));
            }

            List<string>? unUsedButtons = NodeIdsToUse.Except(KeyboardValues.Select(t => t.Key)).ToList(); ;
            foreach (var nodeId in unUsedButtons)
            {
                MsgSender.SendCommand(nodeId, new StandardButtonCommand());
            }

        }


        public virtual void SendEventUpdate(bool gameEnded = false)
        {

            MsgSender.SendUpdate("", gameEnded);
        }

        public void Dispose()
        {
            _isEnabled = false;
            if (subscriptions != null)
            {
                subscriptions.Dispose();
            }
        }



        public void Enable()
        {
            _isEnabled = true;
        }

        public void Disable(bool clearButtons = true)
        {
            _isEnabled = false;
            if (clearButtons)
            {
                MsgSender.ClearButtons();
            }
        }
    }
}
