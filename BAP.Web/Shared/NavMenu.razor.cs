using Microsoft.AspNetCore.Components;

namespace BAP.Web.Shared
{

	public partial class NavMenu
	{
		[Inject]
		IEnumerable<IMainMenuItem> MenuItems { get; set; } = default!;
		[Inject]
		ILoadablePageHandler _LoadableGameHandler { get; set; } = default!;
		[Inject]
		NavigationManager _navigationManager { get; set; } = default!;
		protected async override Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
		}

		private void ActivateItem(IMainMenuItem menuItem)
		{
			_LoadableGameHandler.UpdateCurrentlySelectedItem(menuItem, true);
			_navigationManager.NavigateTo("/");

		}
	}
}
