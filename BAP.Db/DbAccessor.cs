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

namespace BAP.Db
{
    public class DbAccessor
    {
        //Todo - It seems like the DB context should get injected probably from a a factory.
        public async Task<FirmwareInfo> GetLatestFirmwareInfo()
        {
            using ButtonContext db = new();
            return await db.FirmwareInfos.Where(t => t.IsLatestVersion).OrderByDescending(t => t.FirmwareInfoId).FirstOrDefaultAsync() ?? new FirmwareInfo();

        }

        public async Task<List<FirmwareInfo>> GetAllFirmwareInfo()
        {
            using ButtonContext db = new();
            return await db.FirmwareInfos.ToListAsync();

        }

        public async Task<GameFavorite> AddGameFavorite(string unqueId)
        {
            using ButtonContext db = new();
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
            using ButtonContext db = new();
            DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);
            return await db.ButtonLayoutHistories.Where(t => t.DateUsed > thirtyDaysAgo).ToListAsync();
        }

        public async Task<bool> RemoveGameFavorite(string unqueId)
        {
            using ButtonContext db = new();
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
            using ButtonContext db = new();
            return (await db.GamePlayLogs.GroupBy(t => t.GameUniqueId).Select(t => new { uniqueId = t.Key, count = t.Count() }).ToListAsync()).Select(t => (t.uniqueId, t.count)).ToList();

        }

        public async Task<GamePlayLog> AddGamePlayLog(string gameUniqueId)
        {
            using ButtonContext db = new();
            GamePlayLog newLog = new GamePlayLog() { GameUniqueId = gameUniqueId, DateGameSelectedUTC = DateTime.UtcNow };
            db.GamePlayLogs.Add(newLog);
            await db.SaveChangesAsync();
            return newLog;
        }

        public async Task<bool> AddAnyNewMenuItems(List<(string uniqueId, bool showByDefault)> menuItemDetails)
        {
            using ButtonContext db = new();
            HashSet<string> currentMenuItems = (await db.MenuItemStatuses.Select(t => t.MenuItemUniqueId).ToListAsync()).ToHashSet();
            List<string> newItems = menuItemDetails.Select(t => t.uniqueId).Except(currentMenuItems).ToList();
            if (newItems.Count > 0)
            {
                int currentMaxOrder = await db.MenuItemStatuses.MaxAsync(t => t.Order);
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
            using ButtonContext db = new();
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
            using ButtonContext db = new();
            MenuItemStatus? menuItemStatus = await db.MenuItemStatuses.Where(t => t.MenuItemUniqueId == uniqueId).FirstOrDefaultAsync();
            if (menuItemStatus != null && menuItemStatus.ShowInMainMenu == true)
            {
                menuItemStatus.ShowInMainMenu = false;
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<string>> GetUniqueIdsOfActiveMenuItems()
        {
            using ButtonContext db = new();
            return await db.MenuItemStatuses.Where(t => t.ShowInMainMenu).Select(t => t.MenuItemUniqueId).ToListAsync();
        }

        public List<string> AddActiveProvider(Type providerType, bool deactivateOtherProvider)
        {
            using ButtonContext db = new();
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
            using ButtonContext db = new();
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
                    providerName = type.Name;
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The Type of the Interface that you want to Fetch</typeparam>
        /// <returns></returns>
        public List<string> GetRecentlyActiveProvider(string providerInterfaceFullName)
        {

            using ButtonContext db = new();

            return db.ActiveProviders.Where(t => t.ProviderInterfaceFullName == providerInterfaceFullName).Select(t => t.ProviderUniqueId).ToList();
        }

        public async Task<FirmwareInfo> AddLatestFirmware(FirmwareInfo latestFirmwareInfo)
        {
            using ButtonContext db = new();
            List<FirmwareInfo> firmwareInfos = await db.FirmwareInfos.Where(t => t.IsLatestVersion).ToListAsync();
            firmwareInfos.ForEach(t => t.IsLatestVersion = false);
            latestFirmwareInfo.IsLatestVersion = true;
            db.FirmwareInfos.Add(latestFirmwareInfo);
            await db.SaveChangesAsync();
            return latestFirmwareInfo;
        }

        public async Task<ButtonLayoutHistory> AddButtonLayoutHistory(int buttonLayoutId)
        {
            using ButtonContext db = new();
            ButtonLayoutHistory buttonLayoutHistory = new();
            buttonLayoutHistory.ButtonLayoutId = buttonLayoutId;
            buttonLayoutHistory.DateUsed = DateTime.UtcNow;
            db.ButtonLayoutHistories.Add(buttonLayoutHistory);
            await db.SaveChangesAsync();
            return buttonLayoutHistory;
        }


        public async Task<(ButtonLayout buttonLayout, bool newLayourCreated)> AddButtonLayout(List<ButtonPosition> buttonPositions)
        {
            using ButtonContext db = new();
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
            using ButtonContext db = new();
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
            using ButtonContext db = new();
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
