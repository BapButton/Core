using BAP.WebCore;
using Microsoft.AspNetCore.Components;

namespace BAP.WebDev.Shared
{

    public partial class NavMenu
    {
        [Inject]
        LoadedAddonHolder LoadedAddonHolder { get; set; } = default!;
        [Inject]
        NavigationManager _navigationManager { get; set; } = default!;
        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}
