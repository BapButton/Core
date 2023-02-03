using Microsoft.AspNetCore.Components;
using BAP.Web.Games;

namespace BAP.TextGames.Components
{
    public partial class Marquee : ComponentBase, IDisposable
    {
        [Inject]
        IGameProvider GameProvider { get; set; } = default!;

        public void Dispose()
        {

        }

        protected override void OnInitialized()
        {
            GameProvider.UpdateToNewGameType(typeof(TextMarqueGame));
        }
    }
}
