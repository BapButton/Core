using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace BAP.Web
{
    public partial class App
    {
        [Inject]
        LoadedAddonHolder loadedAddons { get; set; } = default!;


    }
}
