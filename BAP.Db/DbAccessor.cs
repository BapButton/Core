using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAP.Types;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Update.Internal;

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

		public async Task<List<string>> AddActiveProvider<T>(bool deactivateOtherProvider) where T : IBapProvider
		{
			using ButtonContext db = new();
			ActiveProvider activeProvider = GenerateActiveProvider<T>();
			List<ActiveProvider> currentActiveProviders = await db.ActiveProviders.Where(t => t.ProviderType == activeProvider.ProviderType).ToListAsync();
			if (currentActiveProviders.Any(t => t.FullName == activeProvider.FullName))
			{
				if (deactivateOtherProvider)
				{
					db.ActiveProviders.RemoveRange(currentActiveProviders.Where(t => t.FullName != activeProvider.FullName));
					await db.SaveChangesAsync();
				}
			}
			else
			{
				if (deactivateOtherProvider)
				{
					db.ActiveProviders.RemoveRange(currentActiveProviders);
				}
				db.ActiveProviders.Add(activeProvider);
				await db.SaveChangesAsync();
			}
			return await GetRecentlyActiveProvider<T>();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="deactivateOtherProvider"></param>
		/// <returns>A List of the Active Providers for that type</returns>
		public async Task<List<string>> RemoveActiveProvider<T>(T type, bool deactivateOtherProvider) where T : IBapProvider
		{
			using ButtonContext db = new();
			ActiveProvider activeProvider = GenerateActiveProvider<T>();
			ActiveProvider? currentProvider = await db.ActiveProviders.FirstOrDefaultAsync(t => t.FullName != activeProvider.FullName && t.ProviderType == activeProvider.ProviderType);
			if (currentProvider != null)
			{
				db.ActiveProviders.Remove(currentProvider);
				await db.SaveChangesAsync();
			}
			return await GetRecentlyActiveProvider<T>();
		}

		private ActiveProvider GenerateActiveProvider<T>() where T : IBapProvider
		{
			ProviderType providerType = ProviderType.Unknown;
			Type t = typeof(T);
			if (t is null)
			{
				throw new Exception("No type was passed in");
			}
			var implementedInterfaces = t.GetInterfaces();
			//todo this really should check what interface it implements and see if it implemthe interface
			if (t is IBapAudioProvider)
			{
				providerType = ProviderType.Audio;
			}
			else if (t is IBapKeyboardProvider)
			{
				providerType = ProviderType.Keyboard;
			}
			else if (t is IBapButtonProvider)
			{
				providerType = ProviderType.Button;
			}
			if (providerType == ProviderType.Unknown)
			{
				throw new Exception("Unknown type {}");
			}
			ActiveProvider activeProvider = new ActiveProvider();
			activeProvider.ProviderType = providerType;
			activeProvider.FullName = t.FullName;
			return activeProvider;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T">The Type of the Interface that you want to Fetch</typeparam>
		/// <returns></returns>
		public async Task<List<string>> GetRecentlyActiveProvider<T>() where T : IBapProvider
		{

			using ButtonContext db = new();
			ActiveProvider activeProvider = GenerateActiveProvider<T>();
			return await db.ActiveProviders.Where(t => t.ProviderType == activeProvider.ProviderType).Select(t => t.FullName).ToListAsync();
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
