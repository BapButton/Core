using Microsoft.AspNetCore.Components;

namespace BAP.WebCore.Components
{

    public partial class SideNavMenu
    {
        [Inject]
        LoadedAddonHolder LoadedAddonHolder { get; set; } = default!;
    }
}
