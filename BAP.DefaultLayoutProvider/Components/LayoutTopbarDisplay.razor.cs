using BAP.Db;
using BAP.Types;
using MessagePipe;
using Microsoft.AspNetCore.Components;

namespace BAP.DefaultLayoutProvider.Components;
[TopMenu()]
public partial class LayoutTopbarDisplay : ComponentBase, IDisposable
{
    [Inject]
    ILayoutProvider LayoutProvider { get; set; } = default!;
    [Inject]
    IBapMessageSender MsgSender { get; set; } = default!;
    [Inject]
    DbAccessor dba { get; set; } = default!;
    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    ISubscriber<NodeChangeMessage> NodeChangedPipe { get; set; } = default!;
    [Inject]
    ISubscriber<LayoutChangeMessage> LayoutChangedPipe { get; set; } = default!;
    IDisposable Subscriptions { get; set; } = default!;
    public List<ButtonLayout> PossibleButtonLayouts = new();
    public List<ButtonLayoutHistory> Last30DaysOfButtonLayoutHistorys = new();

    protected override async Task OnInitializedAsync()
    {

        var bag = DisposableBag.CreateBuilder();

        NodeChangedPipe.Subscribe(async (x) => await ButtonUpdates()).AddTo(bag);
        LayoutChangedPipe.Subscribe(async (x) => await LayoutChanged()).AddTo(bag);
        Subscriptions = bag.Build();
        Last30DaysOfButtonLayoutHistorys = await dba.Last30DaysOfButtonLayouts();
        await LoadPossibleButtonLayouts();
        await SelectOrDeselectAButtonLayoutIfItMakesSense();
    }

    private void NavigateToLayout()
    {
        NavigationManager.NavigateTo("layout");
    }
    async Task ButtonUpdates()
    {
        await SelectOrDeselectAButtonLayoutIfItMakesSense();
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }
    async Task LayoutChanged()
    {
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }

    async Task<bool> SelectOrDeselectAButtonLayoutIfItMakesSense()
    {
        await LoadPossibleButtonLayouts();
        List<int> viableButtonLayoutIds = PossibleButtonLayouts.Select(t => t.ButtonLayoutId).Distinct().ToList();
        if (viableButtonLayoutIds.Count > 0)
        {
            if (!viableButtonLayoutIds.Contains(LayoutProvider.CurrentButtonLayout?.ButtonLayoutId ?? 0))
            {
                LayoutProvider.SetNewButtonLayout(null);
                MsgSender.SendLayoutUpdate(0);
            }
            var bestOption = Last30DaysOfButtonLayoutHistorys.Where(t => viableButtonLayoutIds.Contains(t.ButtonLayoutId)).OrderByDescending(t => t.DateUsed).FirstOrDefault();
            if (bestOption != null)
            {
                var bl = PossibleButtonLayouts.FirstOrDefault(t => t.ButtonLayoutId == bestOption.ButtonLayoutId);
                if (bl != null)
                {
                    LayoutProvider.SetNewButtonLayout(bl);
                    MsgSender.SendLayoutUpdate(bl.ButtonLayoutId);
                }
                return true;
            }
            return false;
        }
        else
        {
            if (LayoutProvider.CurrentButtonLayout != null)
            {
                LayoutProvider.SetNewButtonLayout(null);
                MsgSender.SendLayoutUpdate(0);
            }
        }
        return true;
    }

    private async Task<bool> LoadPossibleButtonLayouts()
    {

        PossibleButtonLayouts = await dba.CurrentlyViableButtonLayouts(MsgSender.GetConnectedButtons().ToHashSet());
        return true;
    }

    public void Dispose()
    {
        Subscriptions.Dispose();
    }
}
