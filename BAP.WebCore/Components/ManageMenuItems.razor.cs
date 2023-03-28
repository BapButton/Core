using BAP.Db;
using BAP.WebCore.Models;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace BAP.WebCore.Components
{

    public partial class ManageMenuItems : ComponentBase, IDisposable
    {

        [Inject]
        DbAccessor dba { get; set; } = default!;
        [Inject]
        LoadedAddonHolder LoadedAddonHolder { get; set; } = default!;
        [Inject]
        IPublisher<MenuLayoutChangedMessage> LayoutChangeProvider { get; set; } = default!;
        List<(MenuItemStatus status, MenuItemDetail menuItemDetail)> Items { get; set; } = new();



        protected override async Task OnInitializedAsync()
        {
            await FetchMenuItems();
        }


        private async Task FetchMenuItems()
        {
            Items.Clear();
            List<MenuItemStatus> menuItemStatuses = await dba.GetMenuItemStatusesAsync();
            foreach (var menuItem in LoadedAddonHolder.MainMenuItems)
            {
                MenuItemStatus? matchingItem = menuItemStatuses.FirstOrDefault(t => t.MenuItemUniqueId == menuItem.UniqueId);
                if (matchingItem != null)
                {
                    Items.Add((matchingItem, menuItem));
                }
            }
        }


        private async Task MarkItemToShowInMenu(string uniqueId)
        {
            await dba.MarkItemAsShowInMenu(uniqueId);
            await FetchMenuItems();
            LayoutChangeProvider.Publish(new MenuLayoutChangedMessage());

        }
        private async Task MarkItemToNotShowInMenu(string uniqueId)
        {
            await dba.MarkItemASDontShowInMenu(uniqueId);
            await FetchMenuItems();
            LayoutChangeProvider.Publish(new MenuLayoutChangedMessage());
        }
        private async Task MoveItemUp(string uniqueId)
        {
            await dba.MoveMenuItemUp(uniqueId);
            await FetchMenuItems();
            LayoutChangeProvider.Publish(new MenuLayoutChangedMessage());
        }
        private async Task MoveItemDown(string uniqueId)
        {
            await dba.MoveMenuItemDown(uniqueId);
            await FetchMenuItems();
            LayoutChangeProvider.Publish(new MenuLayoutChangedMessage());
        }

        public void Dispose()
        {

        }
    }

}
