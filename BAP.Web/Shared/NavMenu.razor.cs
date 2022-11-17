using Microsoft.AspNetCore.Components;

namespace BAP.Web.Shared
{

    public partial class NavMenu
    {
        [Inject]
        IEnumerable<IMainMenuItem> MenuItems { get; set; } = default!;
        [Inject]
        GameHandler _gameHandler { get; set; } = default!;
        [Inject]
        NavigationManager _navigationManager { get; set; } = default!;
        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private void ActivateItem(IMainMenuItem menuItem)
        {
            _gameHandler.UpdateCurrentlySelectedItem(menuItem, true);
            _navigationManager.NavigateTo("/");

        }
    }
}
