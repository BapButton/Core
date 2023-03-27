using BAP.Db;
using BAP.WebCore.Models;
using Microsoft.AspNetCore.Components;

namespace BAP.WebCore.Components
{

    public partial class SideNavMenu : IDisposable
    {
        [Inject]
        LoadedAddonHolder LoadedAddonHolder { get; set; } = default!;
        List<MenuItemDetail> MenuItems { get; set; } = new();
        [Inject]
        DbAccessor dba { get; set; } = default!;
        [Inject]
        ISubscriber<MenuLayoutChangedMessage> MenuChangedPipe { get; set; } = default!;
        IDisposable Subscriptions { get; set; } = default!;



        protected override async Task OnInitializedAsync()
        {
            var bag = DisposableBag.CreateBuilder();
            MenuChangedPipe.Subscribe(async (x) => await UpdateMenuItems()).AddTo(bag);
            Subscriptions = bag.Build();
            await UpdateMenuItems();
        }

        private async Task UpdateMenuItems()
        {
            MenuItems.Clear();
            List<string> activeMenuItems = await dba.GetUniqueIdsOfActiveMenuItems();
            //These items are ordered - so this seems a little inefficient or verbose but it is to get the order right.
            foreach (var uniqueId in activeMenuItems)
            {
                MenuItemDetail? mid = LoadedAddonHolder.MainMenuItems.FirstOrDefault(t => t.UniqueId == uniqueId);
                if (mid != null)
                {
                    MenuItems.Add(mid);
                }
            }
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        public void Dispose()
        {
            Subscriptions.Dispose();
        }

    }
}
