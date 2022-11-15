using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BAP.Db.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FirmwareInfos",
                columns: table => new
                {
                    FirmwareInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FirmwareVersion = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    DateUploaded = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsLatestVersion = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    FileName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirmwareInfos", x => x.FirmwareInfoId);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    ScoreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    UserName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    RelateiveScore = table.Column<int>(type: "int", nullable: false),
                    ScoreInfoJson = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.ScoreId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FirmwareInfos");

            migrationBuilder.DropTable(
                name: "Scores");
        }
    }
}
