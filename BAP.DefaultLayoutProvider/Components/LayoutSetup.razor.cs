﻿namespace BAP.DefaultLayoutProvider.Components;

using BAP.Db;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using BAP.Types;
using BAP.Helpers;
using System.Collections;

[MenuItem("Layout", "Setup Layout for buttons", false, "0aca4ec6-6631-47ca-b54b-c52ea1ba53ec")]
public partial class LayoutSetup : ComponentBase, IDisposable
{
    [Inject]
    IGameProvider GameHandler { get; set; } = default!;
    [Inject]
    ILayoutProvider LayoutProvider { get; set; } = default!;
    [Inject]
    IBapMessageSender MsgSender { get; set; } = default!;
    [Inject]
    DbAccessor dba { get; set; } = default!;

    [Inject]
    ISubscriber<GameEventMessage> GameEventPipe { get; set; } = default!;
    [Inject]
    ISubscriber<LayoutChangeMessage> LayoutChangedPipe { get; set; } = default!;
    [Inject]
    ISubscriber<NodeChangeMessage> NodeChangedPipe { get; set; } = default!;
    IDisposable Subscriptions { get; set; } = default!;
    string LastMessage { get; set; } = "";
    public LayoutSetupGame layoutGame { get; set; } = default!;
    public List<ButtonLayout> PossibleButtonLayouts = new();
    List<int> RowLayout
    {
        get
        {
            if (layoutGame != null)
            {
                return layoutGame?.RowLayout ?? new();

            }
            else
            {
                return new();
            }

        }
    }

    void rerunLayout()
    {
        layoutGame.LayoutButtons();
    }

    private async Task DeleteRow()
    {
        layoutGame.DeleteRow();
        await InvokeAsync(() =>
       {
           StateHasChanged();
       });
    }

    private async Task AddRow()
    {
        layoutGame.AddRow();
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }

    protected override async Task OnInitializedAsync()
    {

        var bag = DisposableBag.CreateBuilder();
        GameEventPipe.Subscribe(async (x) => await Updates(x)).AddTo(bag);
        NodeChangedPipe.Subscribe(async (x) => await ButtonUpdates()).AddTo(bag);
        LayoutChangedPipe.Subscribe(async (x) => await LayoutChanged()).AddTo(bag);
        Subscriptions = bag.Build();
        layoutGame = GameHandler.ReturnGameWithoutEnabling<LayoutSetupGame>();
        await LoadPossibleButtonLayouts();
    }
    async Task ButtonUpdates()
    {
        await LoadPossibleButtonLayouts();
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });

    }
    private async Task<bool> DeleteLayout(ButtonLayout bl)
    {
        await dba.DeleteLayout(bl.ButtonLayoutId);
        await LoadPossibleButtonLayouts();
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
        return true;
    }

    async Task LayoutChanged()
    {
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }
    private async Task<bool> LoadPossibleButtonLayouts()
    {
        PossibleButtonLayouts = await dba.CurrentlyViableButtonLayouts(MsgSender.GetConnectedButtons().ToHashSet());
        return true;
    }

    private async Task<bool> SetButtonLayout(ButtonLayout buttonLayout)
    {
        LayoutProvider.SetNewButtonLayout(buttonLayout);
        await dba.AddButtonLayoutHistory(buttonLayout.ButtonLayoutId);
        MsgSender.SendLayoutUpdate(buttonLayout.ButtonLayoutId);
        return true;
    }

    private void DisplayLayout(ButtonLayout buttonLayout)
    {
        foreach (ButtonPosition bp in buttonLayout.ButtonPositions)
        {
            BapColor? rgb = StandardColorPalettes.Default[bp.RowId];

            MsgSender.SendImage(bp.ButtonId, new ButtonImage(PatternHelper.GetBytesForPattern((Patterns)bp.ColumnId), rgb));
        }
    }


    async Task Updates(GameEventMessage message)
    {
        LastMessage = message.Message;
        if (message.GameEnded)
        {
            await LoadPossibleButtonLayouts();
        }
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }
    async Task Start()
    {

        await layoutGame.Start();
    }

    public void Dispose()
    {
        Subscriptions.Dispose();
        if (GameHandler.IsGameRunning == false)
        {
            MsgSender.SendImageToAllButtons(new ButtonImage());
        }
    }
}
