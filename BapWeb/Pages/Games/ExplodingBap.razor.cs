using Microsoft.AspNetCore.Components;
using BapWeb.Games;

namespace BapWeb.Pages.Games
{
    public partial class ExplodingBap : ComponentBase
    {
        [Inject]
        GameHandler GameHandler { get; set; } = default!;
        protected override void OnInitialized()
        {
            GameHandler.UpdateToNewGameType(typeof(ExplodingBapGame));
        }
    }
}
