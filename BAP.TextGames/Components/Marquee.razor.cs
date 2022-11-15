using Microsoft.AspNetCore.Components;
using BAP.Web.Games;

namespace BAP.TextGames.Components
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
