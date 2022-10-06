using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BapDb.Migrations
{
    public partial class gamefavsAndGameLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayLog_Games_GameId",
                table: "GamePlayLog");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropIndex(
                name: "IX_GamePlayLog_GameId",
                table: "GamePlayLog");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "GamePlayLog");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateGameSelectedUTC",
                table: "GamePlayLog",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "GameUniqueId",
                table: "GamePlayLog",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GameFavorites",
                columns: table => new
                {
                    GameFavoriteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameUniqueId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsFavorite = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameFavorites", x => x.GameFavoriteId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GameFavorites_GameUniqueId",
                table: "GameFavorites",
                column: "GameUniqueId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameFavorites");

            migrationBuilder.DropColumn(
                name: "DateGameSelectedUTC",
                table: "GamePlayLog");

            migrationBuilder.DropColumn(
                name: "GameUniqueId",
                table: "GamePlayLog");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "GamePlayLog",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsFavorite = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayLog_GameId",
                table: "GamePlayLog",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayLog_Games_GameId",
                table: "GamePlayLog",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
