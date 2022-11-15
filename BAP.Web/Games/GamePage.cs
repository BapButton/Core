
using Microsoft.AspNetCore.Components;
using System.Threading;
using BAP.Types;

namespace BAP.Web.Games;
public abstract class GamePage : ComponentBase, IGamePage, IDisposable
{
    CancellationTokenSource timerUpdator = new();
    [Inject]
    public IDialogService DialogService { get; set; } = default!;

    public async virtual Task<bool> NodesChangedAsync(NodeChangeMessage nodeChangeMessage)
    {
        return await Task.FromResult(true);
    }

    public async virtual Task<bool> LayoutChangedAsync()
    {
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
        return true;
    }

    public async virtual Task<bool> GameUpdateAsync(GameEventMessage gameEventMessage)
    {
        if (gameEventMessage.PageRefreshRecommended)
        {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        return true;
    }

    internal async Task KeepTimeUpdated()
    {
        if (timerUpdator.IsCancellationRequested)
        {
            timerUpdator = new();
        }
        var timerToken = timerUpdator.Token;
        var second = TimeSpan.FromSeconds(1);
        using var timer = new PeriodicTimer(second);
        while (await timer.WaitForNextTickAsync(timerToken))
        {
            await InvokeAsync(StateHasChanged);
        };

    }
    internal void StopUpdatingTime()
    {
        if (timerUpdator != null && !timerUpdator.IsCancellationRequested)
        {
            timerUpdator.Cancel();
        }

    }
    public virtual void Dispose()
    {
        timerUpdator.Dispose();
    }
}
