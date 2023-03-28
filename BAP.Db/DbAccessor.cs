using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAP.Types;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Buffers;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Internal;
using System.Xml;

namespace BAP.Db
{
    public class DbAccessor
    {
        private readonly IDbContextFactory<ButtonContext> _contextFactory;
        public DbAccessor(IDbContextFactory<ButtonContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ButtonContext GetButtonContext()
        {
            return _contextFactory.CreateDbContext();
        }


        public async Task<FirmwareInfo> GetLatestFirmwareInfo()
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            return await db.FirmwareInfos.Where(t => t.IsLatestVersion).OrderByDescending(t => t.FirmwareInfoId).FirstOrDefaultAsync() ?? new FirmwareInfo();

        }

        public async Task<List<FirmwareInfo>> GetAllFirmwareInfo()
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            return await db.FirmwareInfos.ToListAsync();

        }

        public async Task<GameFavorite> AddGameFavorite(string unqueId)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            GameFavorite? gameFavorite = await db.GameFavorites.FirstOrDefaultAsync(t => t.GameUniqueId == unqueId);
            if (gameFavorite == null)
            {
                gameFavorite = new GameFavorite()
                {
                    GameUniqueId = unqueId,
                    IsFavorite = true
                };
                db.GameFavorites.Add(gameFavorite);
            };
            await db.SaveChangesAsync();
            return gameFavorite;
        }

        public async Task<List<ButtonLayoutHistory>> Last30DaysOfButtonLayouts()
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);
            return await db.ButtonLayoutHistories.Where(t => t.DateUsed > thirtyDaysAgo).ToListAsync();
        }

        public async Task<bool> RemoveGameFavorite(string unqueId)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            bool favoriteFound = false;
            GameFavorite? gameFavorite = await db.GameFavorites.FirstOrDefaultAsync(t => t.GameUniqueId == unqueId);
            if (gameFavorite != null)
            {
                db.GameFavorites.Remove(gameFavorite);
                favoriteFound = true;
            };
            await db.SaveChangesAsync();
            return favoriteFound;
        }

        public async Task<List<(string uniqueId, int playCount)>> GetGamePlayStatistics()
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            return (await db.GamePlayLogs.GroupBy(t => t.GameUniqueId).Select(t => new { uniqueId = t.Key, count = t.Count() }).ToListAsync()).Select(t => (t.uniqueId, t.count)).ToList();

        }

        public async Task<GamePlayLog> AddGamePlayLog(string gameUniqueId)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            GamePlayLog newLog = new GamePlayLog() { GameUniqueId = gameUniqueId, DateGameSelectedUTC = DateTime.UtcNow };
            db.GamePlayLogs.Add(newLog);
            await db.SaveChangesAsync();
            return newLog;
        }

        public async Task<bool> AddAnyNewMenuItems(List<(string uniqueId, bool showByDefault)> menuItemDetails)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            List<MenuItemStatus> menuItemStatuses = await db.MenuItemStatuses.ToListAsync();
            HashSet<string> currentMenuItems = (menuItemStatuses.Select(t => t.MenuItemUniqueId).ToList()).ToHashSet();
            List<string> newItems = menuItemDetails.Select(t => t.uniqueId).Except(currentMenuItems).ToList();
            if (newItems.Count > 0)
            {
                int currentMaxOrder = 0;
                if (currentMenuItems.Any())
                {
                    currentMaxOrder = menuItemStatuses.Select(t => t.Order).Max();
                }
                foreach (var item in newItems)
                {
                    currentMaxOrder++;
                    var menuItem = menuItemDetails.First(t => t.uniqueId == item);
                    db.MenuItemStatuses.Add(new MenuItemStatus() { MenuItemUniqueId = item, Order = currentMaxOrder, ShowInMainMenu = menuItem.showByDefault });
                }
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> MarkItemAsShowInMenu(string uniqueId)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            MenuItemStatus? menuItemStatus = await db.MenuItemStatuses.Where(t => t.MenuItemUniqueId == uniqueId).FirstOrDefaultAsync();
            if (menuItemStatus != null && menuItemStatus.ShowInMainMenu == false)
            {
                menuItemStatus.ShowInMainMenu = true;
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> MarkItemASDontShowInMenu(string uniqueId)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            MenuItemStatus? menuItemStatus = await db.MenuItemStatuses.Where(t => t.MenuItemUniqueId == uniqueId).FirstOrDefaultAsync();
            if (menuItemStatus != null && menuItemStatus.ShowInMainMenu == true)
            {
                menuItemStatus.ShowInMainMenu = false;
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<MenuItemStatus>> GetMenuItemStatusesAsync()
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            return await db.MenuItemStatuses.ToListAsync();
        }

        public async Task MoveMenuItemUp(string uniqueId)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            List<MenuItemStatus> menuItems = await db.MenuItemStatuses.OrderBy(t => t.Order).ToListAsync();
            MenuItemStatus? itemToAdjust = menuItems.FirstOrDefault(t => t.MenuItemUniqueId.Equals(uniqueId));
            if (itemToAdjust != null)
            {
                int originalOrder = itemToAdjust.Order;
                int placeInLine = menuItems.IndexOf(itemToAdjust);
                if (placeInLine > 0)
                {
                    MenuItemStatus itemToSwap = menuItems[placeInLine - 1];
                    itemToAdjust.Order = itemToSwap.Order;
                    itemToSwap.Order = originalOrder;
                    await db.SaveChangesAsync();
                }

            }
        }

        public async Task MoveMenuItemDown(string uniqueId)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            List<MenuItemStatus> menuItems = await db.MenuItemStatuses.OrderBy(t => t.Order).ToListAsync();
            MenuItemStatus? itemToAdjust = menuItems.FirstOrDefault(t => t.MenuItemUniqueId.Equals(uniqueId));
            if (itemToAdjust != null)
            {
                int originalOrder = itemToAdjust.Order;
                int placeInLine = menuItems.IndexOf(itemToAdjust);
                if (placeInLine < menuItems.Count)
                {
                    MenuItemStatus itemToSwap = menuItems[placeInLine + 1];
                    itemToAdjust.Order = itemToSwap.Order;
                    itemToSwap.Order = originalOrder;
                    await db.SaveChangesAsync();
                }

            }
        }

        public async Task<List<string>> GetUniqueIdsOfActiveMenuItems()
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            return await db.MenuItemStatuses.Where(t => t.ShowInMainMenu).OrderBy(t => t.Order).Select(t => t.MenuItemUniqueId).ToListAsync();
        }

        public List<string> AddActiveProvider(Type providerType, bool deactivateOtherProvider)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            ActiveProvider? activeProvider = GenerateActiveProvider(providerType);
            if (activeProvider != null)
            {
                List<ActiveProvider> currentActiveProviders = db.ActiveProviders.Where(t => t.ProviderInterfaceFullName == activeProvider.ProviderInterfaceFullName).ToList();
                if (currentActiveProviders.Any(t => t.ProviderUniqueId == activeProvider.ProviderUniqueId))
                {
                    if (deactivateOtherProvider)
                    {
                        db.ActiveProviders.RemoveRange(currentActiveProviders.Where(t => t.ProviderUniqueId != activeProvider.ProviderUniqueId));
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (deactivateOtherProvider)
                    {
                        db.ActiveProviders.RemoveRange(currentActiveProviders);
                    }
                    db.ActiveProviders.Add(activeProvider);
                    db.SaveChanges();
                }
            }
            if (activeProvider != null)
            {
                return GetRecentlyActiveProvider(activeProvider.ProviderInterfaceFullName);
            }
            return new();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="deactivateOtherProvider"></param>
        /// <returns>A List of the Active Providers for that type</returns>
        public async Task<List<string>> RemoveActiveProvider(Type providerToRemoveType, bool deactivateOtherProvider)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            ActiveProvider? activeProvider = GenerateActiveProvider(providerToRemoveType);
            if (activeProvider != null)
            {
                ActiveProvider? currentProvider = await db.ActiveProviders.FirstOrDefaultAsync(t => t.ProviderUniqueId != activeProvider.ProviderUniqueId && t.ProviderInterfaceFullName == activeProvider.ProviderInterfaceFullName);
                if (currentProvider != null)
                {
                    db.ActiveProviders.Remove(currentProvider);
                    await db.SaveChangesAsync();
                }
            }

            return GetRecentlyActiveProvider(activeProvider.ProviderInterfaceFullName);
        }

        private ActiveProvider? GenerateActiveProvider(Type providerType)
        {

            string providerName = "";
            if (providerType is null)
            {
                throw new Exception("No type was passed in");
            }
            var implementedInterfaces = providerType.GetInterfaces();
            foreach (Type type in implementedInterfaces)
            {
                if (type.GetCustomAttribute<BapProviderInterfaceAttribute>() != null)
                {
                    providerName = type?.FullName ?? type?.Name ?? "";
                    break;
                }
            }
            var bapProviderAttribute = providerType.GetCustomAttribute<BapProviderAttribute>();
            if (bapProviderAttribute != null)
            {
                ActiveProvider activeProvider = new ActiveProvider();
                activeProvider.ProviderInterfaceFullName = providerName;
                activeProvider.ProviderUniqueId = bapProviderAttribute.UniqueId;
                activeProvider.ProviderName = bapProviderAttribute.Name;
                return activeProvider;
            }

            return null;
        }


        public List<string> GetRecentlyActiveProvider(string providerInterfaceFullName)
        {
            try
            {
                using ButtonContext db = _contextFactory.CreateDbContext();

                return db.ActiveProviders.Where(t => t.ProviderInterfaceFullName == providerInterfaceFullName).Select(t => t.ProviderUniqueId).ToList();
            }
            catch (Exception ex)
            {

                return new List<string>();
            }

        }

        public async Task<FirmwareInfo> AddLatestFirmware(FirmwareInfo latestFirmwareInfo)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            List<FirmwareInfo> firmwareInfos = await db.FirmwareInfos.Where(t => t.IsLatestVersion).ToListAsync();
            firmwareInfos.ForEach(t => t.IsLatestVersion = false);
            latestFirmwareInfo.IsLatestVersion = true;
            db.FirmwareInfos.Add(latestFirmwareInfo);
            await db.SaveChangesAsync();
            return latestFirmwareInfo;
        }

        public async Task<ButtonLayoutHistory> AddButtonLayoutHistory(int buttonLayoutId)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            ButtonLayoutHistory buttonLayoutHistory = new();
            buttonLayoutHistory.ButtonLayoutId = buttonLayoutId;
            buttonLayoutHistory.DateUsed = DateTime.UtcNow;
            db.ButtonLayoutHistories.Add(buttonLayoutHistory);
            await db.SaveChangesAsync();
            return buttonLayoutHistory;
        }


        public async Task<(ButtonLayout buttonLayout, bool newLayourCreated)> AddButtonLayout(List<ButtonPosition> buttonPositions)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            ButtonLayout buttonLayout = new()
            {
                ColumnCount = buttonPositions.Select(t => t.ColumnId).Distinct().Count(),
                RowCount = buttonPositions.Select(t => t.RowId).Distinct().Count(),
                TotalButtons = buttonPositions.Count
            };
            buttonLayout.ButtonPositions = buttonPositions;
            List<ButtonLayout> buttonLayoutsWithSameNumberOfButtons = await db.ButtonLayouts.Where(t => t.TotalButtons == buttonLayout.TotalButtons).Include(t => t.ButtonPositions).ToListAsync();
            string includedButtons = string.Join(",", buttonPositions.OrderBy(t => t.ButtonId).Select(t => t.ButtonId));
            ButtonLayout? existingButtonLayout = null;
            foreach (var layout in buttonLayoutsWithSameNumberOfButtons)
            {
                if (includedButtons.Equals(string.Join(",", layout.ButtonPositions.OrderBy(t => t.ButtonId).Select(t => t.ButtonId)), StringComparison.OrdinalIgnoreCase))
                {
                    bool misMatchFound = false;
                    foreach (var bp in layout.ButtonPositions)
                    {
                        if (buttonPositions.FirstOrDefault(t => t.ButtonId == bp.ButtonId && t.RowId == bp.RowId && t.ColumnId == bp.ColumnId) == null)
                        {
                            misMatchFound = true;
                            break;
                        }
                    }
                    if (misMatchFound == false)
                    {
                        existingButtonLayout = layout;
                        break;
                    }
                }
            }
            if (existingButtonLayout == null)
            {
                db.ButtonLayouts.Add(buttonLayout);
                await db.SaveChangesAsync();
                return (buttonLayout, true);
            }
            else
            {
                return (existingButtonLayout, false);
            }


        }

        public async Task<bool> DeleteLayout(int buttonLayoutId)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            db.RemoveRange(db.ButtonPositions.Where(t => t.ButtonLayoutId == buttonLayoutId));
            ButtonLayout? bl = await db.ButtonLayouts.FirstOrDefaultAsync(t => t.ButtonLayoutId == buttonLayoutId);
            if (bl != null)
            {
                db.Remove(bl);
            }

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<List<ButtonLayout>> CurrentlyViableButtonLayouts(HashSet<string> currentButtonList)
        {
            using ButtonContext db = _contextFactory.CreateDbContext();
            List<ButtonLayout> viableButtonLayouts = new List<ButtonLayout>();
            List<ButtonLayout> allButtonLayouts = await db.ButtonLayouts.Where(t => t.ButtonPositions.Count() == currentButtonList.Count()).Include(t => t.ButtonPositions).ToListAsync();
            foreach (ButtonLayout bl in allButtonLayouts)
            {
                bool mismatchFound = false;
                foreach (ButtonPosition bp in bl.ButtonPositions)
                {
                    if (!currentButtonList.Contains(bp.ButtonId))
                    {
                        mismatchFound = true;
                        break;
                    }
                }
                if (mismatchFound == false)
                {
                    viableButtonLayouts.Add(bl);
                }
            }
            return viableButtonLayouts;
        }

    }
}
