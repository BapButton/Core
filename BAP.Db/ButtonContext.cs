using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using BAP.Types;

namespace BAP.Db
{
	public class ButtonContext : DbContext
	{
		public DbSet<FirmwareInfo> FirmwareInfos => Set<FirmwareInfo>();
		public DbSet<Score> Scores => Set<Score>();
		public DbSet<GameStorage> GameStorageVault => Set<GameStorage>();
		public DbSet<GameFavorite> GameFavorites => Set<GameFavorite>();
		public DbSet<GamePlayLog> GamePlayLogs => Set<GamePlayLog>();
		public DbSet<ButtonLayout> ButtonLayouts => Set<ButtonLayout>();
		public DbSet<ButtonPosition> ButtonPositions => Set<ButtonPosition>();
		public DbSet<ButtonLayoutHistory> ButtonLayoutHistories => Set<ButtonLayoutHistory>();
		public DbSet<ActiveProvider> ActiveProviders => Set<ActiveProvider>();
		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			string conectionString = Environment.GetEnvironmentVariable("DbConnectionString") ?? "";
			if (!options.IsConfigured)
			{
				if (string.IsNullOrEmpty(conectionString))
				{
					options.UseInMemoryDatabase(new Guid().ToString());
				}
				else
				{
					options.UseMySql(conectionString, MariaDbServerVersion.LatestSupportedServerVersion);
				}
			}


		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<GameFavorite>().HasIndex(p => p.GameUniqueId).IsUnique();
			modelBuilder.Entity<GameStorage>().HasIndex(p => p.GameUniqueId).IsUnique();

			var dateTimeConverter = new ValueConverter<DateTime, DateTime>(dateValue => dateValue, dateValue => DateTime.SpecifyKind(dateValue, DateTimeKind.Utc));
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				foreach (var property in entityType.GetProperties())
				{
					if ((property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?)) && property.Name.Contains("Utc", StringComparison.InvariantCultureIgnoreCase))
					{
						property.SetValueConverter(dateTimeConverter);
					}
				}
			}

		}
	}


}
