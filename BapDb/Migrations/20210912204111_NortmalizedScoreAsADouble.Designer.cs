﻿// <auto-generated />
using System;
using BapDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BapDb.Migrations
{
    [DbContext(typeof(ButtonContext))]
    [Migration("20210912204111_NortmalizedScoreAsADouble")]
    partial class NortmalizedScoreAsADouble
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "6.0.0-preview.7.21378.4");

            modelBuilder.Entity("BapDb.FirmwareInfo", b =>
                {
                    b.Property<int>("FirmwareInfoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("DateUploaded")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FirmwareVersion")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsLatestVersion")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Md5Hash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("FirmwareInfoId");

                    b.ToTable("FirmwareInfos");
                });

            modelBuilder.Entity("BapDb.GameFavorite", b =>
                {
                    b.Property<int>("GameFavoriteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("GameUniqueId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.Property<bool>("IsFavorite")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("GameFavoriteId");

                    b.HasIndex("GameUniqueId")
                        .IsUnique();

                    b.ToTable("GameFavorites");
                });

            modelBuilder.Entity("BapDb.GamePlayLog", b =>
                {
                    b.Property<int>("GamePlayLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("DateGameSelectedUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("GameUniqueId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.HasKey("GamePlayLogId");

                    b.ToTable("GamePlayLogs");
                });

            modelBuilder.Entity("BapDb.GameStorage", b =>
                {
                    b.Property<int>("GameStorageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("GameUniqueId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("GameStorageId");

                    b.HasIndex("GameUniqueId")
                        .IsUnique();

                    b.ToTable("GameStorageVault");
                });

            modelBuilder.Entity("BapDb.Score", b =>
                {
                    b.Property<int>("ScoreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ButtonCount")
                        .HasColumnType("int");

                    b.Property<string>("GameId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("NormalizedScore")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("OtherScoringFactors")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ScoreData")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ScoreDescription")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ScoringModelVersion")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ScoreId");

                    b.ToTable("Scores");
                });

            modelBuilder.Entity("BapShared.ButtonLayout", b =>
                {
                    b.Property<int>("ButtonLayoutId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ColumnCount")
                        .HasColumnType("int");

                    b.Property<int>("RowCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalButtons")
                        .HasColumnType("int");

                    b.HasKey("ButtonLayoutId");

                    b.ToTable("ButtonLayouts");
                });

            modelBuilder.Entity("BapShared.ButtonLayoutHistory", b =>
                {
                    b.Property<int>("ButtonLayoutHistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ButtonLayoutId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateUsed")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ButtonLayoutHistoryId");

                    b.HasIndex("ButtonLayoutId");

                    b.ToTable("ButtonLayoutHistories");
                });

            modelBuilder.Entity("BapShared.ButtonPosition", b =>
                {
                    b.Property<int>("ButtonPositionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ButtonId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<int>("ButtonLayoutId")
                        .HasColumnType("int");

                    b.Property<int>("ColumnId")
                        .HasColumnType("int");

                    b.Property<int>("RowId")
                        .HasColumnType("int");

                    b.HasKey("ButtonPositionId");

                    b.HasIndex("ButtonLayoutId");

                    b.ToTable("ButtonPositions");
                });

            modelBuilder.Entity("BapShared.ButtonLayoutHistory", b =>
                {
                    b.HasOne("BapShared.ButtonLayout", "ButtonLayout")
                        .WithMany("ButtonLayoutHistories")
                        .HasForeignKey("ButtonLayoutId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ButtonLayout");
                });

            modelBuilder.Entity("BapShared.ButtonPosition", b =>
                {
                    b.HasOne("BapShared.ButtonLayout", "ButtonLayout")
                        .WithMany("ButtonPositions")
                        .HasForeignKey("ButtonLayoutId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ButtonLayout");
                });

            modelBuilder.Entity("BapShared.ButtonLayout", b =>
                {
                    b.Navigation("ButtonLayoutHistories");

                    b.Navigation("ButtonPositions");
                });
#pragma warning restore 612, 618
        }
    }
}
