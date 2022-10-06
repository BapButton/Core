
using BapDb;
using MessagePipe;
using BapWeb.Pages;
using BapButton;
using BapShared;

namespace BapWeb.Games;

public class LayoutSetupGame : ITinkerGame
{

    public bool IsGameRunning { get; set; }
    public ILogger _logger { get; set; }
    private List<ButtonPosition> ButtonPositions { get; set; } = new();
    public ButtonLayout CreatedButtonLayout { get; set; }
    public DbAccessor dba { get; set; }
    public MessageSender _msgSender { get; internal set; }
    public List<int> RowLayout { get; internal set; } = new();
    public int RowCount { get; internal set; } = 2;
    public int CurrentRow { get; internal set; } = 1;
    public int CurrentColumn { get; internal set; } = 1;

    ISubscriber<ButtonPressedMessage> ButtonPressedPipe { get; set; } = default!;
    IDisposable subscriptions = default!;

    public LayoutSetupGame(DbAccessor dbaccessor, ILogger<KeyboardBase> logger, ISubscriber<ButtonPressedMessage> buttonPressed, MessageSender msgSender)
    {
        _logger = logger;
        ButtonPressedPipe = buttonPressed;
        _msgSender = msgSender;
        var bag = DisposableBag.CreateBuilder();
        ButtonPressedPipe.Subscribe(async (x) => await OnButtonPressed(x)).AddTo(bag);
        subscriptions = bag.Build();
        dba = dbaccessor;


    }

    public void AddRow()
    {
        RowCount++;
        LayoutButtons();
    }
    public void DeleteRow()
    {
        RowCount--;
        LayoutButtons();
    }

    public void LayoutButtons()
    {

        int connectedButtons = _msgSender.GetConnectedButtons().Count;

        if (connectedButtons > 0)
        {
            RowLayout = new();
            int minNumberPerRow = (int)Math.Floor((double)connectedButtons / (double)RowCount); // 12
            int remainder = connectedButtons % RowCount;
            for (int i = 0; i < RowCount; i++)
            {
                if (i < remainder)
                {
                    RowLayout.Add(minNumberPerRow + 1);
                }
                else
                {
                    RowLayout.Add(minNumberPerRow);
                }
            }
        }
    }


    public async Task OnButtonPressed(ButtonPressedMessage e)
    {
        if (IsGameRunning)
        {
            if (!ButtonPositions.Select(t => t.ButtonId).Contains(e.NodeId))
            {
                _msgSender.StopAllAudio();
                ButtonPositions.Add(new() { ColumnId = CurrentColumn, RowId = CurrentRow, ButtonId = e.NodeId });
                _logger.LogDebug($"Node {e.NodeId} added in Row {CurrentRow} and Column {CurrentColumn}");
                BapColor? rgb = StandardColorPalettes.Default[CurrentRow];
                _msgSender.SendCommand(e.NodeId, new StandardButtonCommand(new ButtonDisplay(rgb.Red, rgb.Green, rgb.Blue, (Patterns)CurrentColumn),
                    new ButtonDisplay(rgb.Red, rgb.Green, rgb.Blue, (Patterns)CurrentColumn)));

                if (CurrentColumn == RowLayout[CurrentRow - 1] && RowLayout.Count != CurrentRow)
                {
                    CurrentRow++;
                    CurrentColumn = 1;
                    _msgSender.StopAllAudio();
                    _msgSender.PlayTTS($"Moving to the {AddOrdinal(CurrentRow)} row. Press the first button on the left.");
                }
                else
                {
                    CurrentColumn++;
                }
            }
            else
            {
                _logger.LogDebug($"Node {e.NodeId} was already added");
            }
            if (_msgSender.GetConnectedButtons().Count() == ButtonPositions.Count())
            {
                var result = await dba.AddButtonLayout(ButtonPositions);
                if (result.newLayourCreated)
                {
                    _msgSender.PlayTTS("Setup Complete");
                }
                else
                {
                    _msgSender.PlayTTS("Setup Complete - layout already existed");
                }
                await EndGame("Layout is setup and saved");
            }
            else
            {
                _msgSender.StopAllAudio();
                if (CurrentColumn > 1)
                {
                    _msgSender.StopAllAudio();
                    _msgSender.PlayTTS("Next Button to the right");
                }

            }

        }
    }
    public Task<bool> ForceEndGame()
    {
        return EndGame("Game force Ended");
    }

    public Task<bool> EndGame(string message)
    {
        IsGameRunning = false;
        _msgSender.SendUpdate("Layout Setup Completed", true);
        return Task.FromResult(true);
    }

    public static string AddOrdinal(int num)
    {
        if (num <= 0) return num.ToString();

        switch (num % 100)
        {
            case 11:
            case 12:
            case 13:
                return num + "th";
        }

        switch (num % 10)
        {
            case 1:
                return num + "st";
            case 2:
                return num + "nd";
            case 3:
                return num + "rd";
            default:
                return num + "th";
        }
    }
    public Task<bool> Start()
    {
        CurrentRow = 1;
        CurrentColumn = 1;
        CreatedButtonLayout = new ButtonLayout();
        ButtonPositions = new List<ButtonPosition>();
        _msgSender.SendGeneralCommand(new StandardButtonCommand());
        _msgSender.StopAllAudio();
        if (RowLayout.Sum() != _msgSender.GetConnectedButtons().Count())
        {
            _msgSender.SendUpdate($"Row layout has {RowLayout.Sum()} buttons but there are {_msgSender.GetConnectedButtons().Count()} connected. They must match");
            _msgSender.PlayTTS("Mismatch between Row layout and Button Count. Please Correct.");
        }
        else
        {
            IsGameRunning = true;
            _msgSender.PlayTTS($"Press the left most button in the {AddOrdinal(1)} row");
        }

        return Task.FromResult(true);
    }

    public void Dispose()
    {
        IsGameRunning = false;
        subscriptions.Dispose();
    }
}
