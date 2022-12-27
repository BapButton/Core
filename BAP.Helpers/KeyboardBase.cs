using MessagePipe;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BAP.Types;
using System.Collections.Concurrent;
using ConcurrentCollections;

namespace BAP.Helpers;

public abstract class KeyboardBase : IKeyboardProvider
{
    public ILogger _logger { get; set; }
    private List<string> NodeIdsToUse { get; set; } = new();
    public abstract string Name { get; }
    public Dictionary<string, char> KeyboardValues = new();
    public IBapMessageSender MsgSender { get; internal set; }
    private IPublisher<KeyboardKeyPressedMessage> KeyboardKeyPressedSender { get; set; } = default!;
    private BapColor ColorForCharacters { get; set; } = default!;
    private int CurrentPlaceInArray { get; set; }
    private char CurrentCorrectCharId { get; set; }
    private char leftArrow = '←';
    private char rightArrow = '→';
    private bool moveBackwards = false;
    private bool AlwaysUseAllButtons = true;
    private bool PlayKeyPressSound = false;

    ISubscriber<ButtonPressedMessage> ButtonPressedPipe { get; set; } = default!;
    IDisposable subscriptions = default!;
    ConcurrentHashSet<string> NodeIdsToAvoid = new();
    List<Char> ValuesToDisplay = new();
    public int CurrentNumber { get; set; }

    public bool IsEnabled
    {
        get
        {
            return _isEnabled;
        }
    }
    private bool _isEnabled { get; set; }

    private bool _isConfigured { get; set; }

    public BapColor DisplayColor => ColorForCharacters;

    public List<string> NodesToAvoid => NodeIdsToAvoid.ToList();

    public List<string> ActiveNodes => NodeIdsToUse.ToList();

    public bool PlayDefaultSoundOnPress => PlayKeyPressSound;

    public bool AllowMultiple { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public KeyboardBase(ILogger logger, ISubscriber<ButtonPressedMessage> buttonPressed, IBapMessageSender msgSender, IPublisher<KeyboardKeyPressedMessage> keyboardKeyPressedSender)
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

    public virtual async void ShowKeyboard(int turnOnInMillis = 0)
    {
        Enable();
        if (turnOnInMillis > 0)
        {
            await Task.Delay(turnOnInMillis);
        }
        //I should really check if all of the buttons are in use and then use general command;
        foreach (var keyAndNodeId in KeyboardValues)
        {
            bool isArrow = keyAndNodeId.Value == rightArrow || keyAndNodeId.Value == leftArrow;
            ButtonImage main = new(PatternHelper.GetBytesForPattern(EnumHelper.GetEnumFromCharacter(keyAndNodeId.Value)), ColorForCharacters);
            //ButtonDisplay onPress = new(ColorForCharacters, EnumHelper.GetEnumFromCharacter(keyAndNodeId.Value));
            //ButtonDisplay onPress = isArrow ? initial : DefaultShowOnPress ?? initial;
            //if (CurrentCorrectCharId == keyAndNodeId.Value)
            //{
            //    onPress = ShowOnCorrectPress ?? DefaultShowOnPress ?? initial;
            //}
            //if (onPress.TurnOffAfterMillis == 0)
            //{
            //    onPress.TurnOffAfterMillis = 500;
            //}
            MsgSender.SendImage(keyAndNodeId.Key, main);
        }

        List<string>? unUsedButtons = NodeIdsToUse.Except(KeyboardValues.Select(t => t.Key)).ToList(); ;
        foreach (var nodeId in unUsedButtons)
        {
            MsgSender.SendImage(nodeId, new ButtonImage());
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

    public void SetCharacters(string characters)
    {
        ValuesToDisplay = characters.ToCharArray().ToList();
    }

    public void Reset()
    {
        KeyboardValues = new Dictionary<string, char>();
        CurrentPlaceInArray = 0;
        _isConfigured = true;
        moveBackwards = false;
        CurrentCorrectCharId = default;
        NodeIdsToUse = MsgSender.GetConnectedButtonsInOrder().Except(NodeIdsToAvoid).ToList();
        LayoutKeyboard();
    }

    public void SetColor(BapColor color)
    {
        ColorForCharacters = color;
    }

    public void AddNodesToAvoid(List<string> nodeIds)
    {
        foreach (var nodeId in nodeIds)
        {
            NodeIdsToAvoid.Add(nodeId);
        }
    }

    public void RemoveNodesToAvoid(List<string> nodeIds)
    {
        foreach (var nodeId in nodeIds)
        {
            NodeIdsToAvoid.TryRemove(nodeId);
        }
    }

    public void SetPlayDefaultSoundOnPress(bool playDefaultSoundOnPress)
    {
        PlayKeyPressSound = playDefaultSoundOnPress;
    }

    public void OverrideButtonWithImage(char character, ButtonImage buttonImage, int timoutInMillis)
    {
        var nodeId = GetNodeIdByChar(character);
    }

    public Task<bool> InitializeAsync()
    {

        Reset();
        return Task.FromResult(true);

    }
}
