using Microsoft.AspNetCore.Components;
using BapWeb.Games;

namespace BapWeb.Pages
{
    public partial class Marquee : ComponentBase, IDisposable
    {
        [Inject]
        GameHandler GameHandler { get; set; } = default!;

        public void Dispose()
        {

        }

        protected override void OnInitialized()
        {
            GameHandler.UpdateToNewGameType(typeof(TextMarqueGame));
        }
    }
}
