
using Microsoft.AspNetCore.Components;

namespace BapWeb.Pages;
public partial class ButtonMessageLog : ComponentBase, IDisposable
{
    [Inject]
    ISubscriber<ButtonPressedMessage> ButtonPressedPipe { get; set; } = default!;
    [Inject]
    ISubscriber<StandardButtonCommandMessage> ButtonCommandMessage { get; set; } = default!;

    IDisposable subscriptions = default!;
    private int itemNumber = 0;

    private static FixedSizedQueue<(int itemNumber, bool isButtonPressed, string message, DateTime timeRecievedUTC)> messageQueue { get; set; } = new(100);
    protected override void OnInitialized()
    {
        base.OnInitialized();
        var bag = DisposableBag.CreateBuilder();
        ButtonPressedPipe.Subscribe(async (x) => await ButtonPressed(x)).AddTo(bag);
        ButtonCommandMessage.Subscribe(async (x) => await ButtonCommand(x)).AddTo(bag);
        subscriptions = bag.Build();
    }

    public async Task Clear()
    {
        messageQueue.Clear();
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }

    public async Task<bool> ButtonPressed(ButtonPressedMessage bp)
    {
        string message = $"Button {bp.NodeId} pressed with {bp.ButtonPress}";
        messageQueue.Enqueue((itemNumber, true, message, DateTime.UtcNow));
        itemNumber++;
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
        return await Task.FromResult(true);
    }

    public async Task<bool> ButtonCommand(StandardButtonCommandMessage bcm)
    {
        string message = string.IsNullOrEmpty(bcm.NodeId) ? "All Buttons " : $"Button {bcm.NodeId} ";
        message += bcm.StandardButtonMessage.ToString() ?? "";
        messageQueue.Enqueue((itemNumber, false, message, DateTime.UtcNow));
        itemNumber++;
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
        return await Task.FromResult(true);
    }

    public void Dispose()
    {
        subscriptions.Dispose();
    }
}
