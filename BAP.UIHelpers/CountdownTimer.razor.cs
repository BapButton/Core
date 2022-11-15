
using Microsoft.AspNetCore.Components;

namespace BAP.Web.Pages;
public partial class CountdownTimer : ComponentBase, IDisposable
{
    [Parameter]
    public TimeSpan TimeRemaining { get; set; }
    [Parameter]
    public string Description { get; set; } = "Time Remaining";

    public void Dispose()
    {

    }
}
