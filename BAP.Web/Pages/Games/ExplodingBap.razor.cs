using Microsoft.AspNetCore.Components;
using BAP.Web.Games;

namespace BAP.Web.Pages.Games
{
    public partial class ExplodingBap : ComponentBase
    {
        [Inject]
        IGameProvider GameHandler { get; set; } = default!;
        protected override void OnInitialized()
        {
            GameHandler.UpdateToNewGameType(typeof(ExplodingBapGame));
        }
    }
}
