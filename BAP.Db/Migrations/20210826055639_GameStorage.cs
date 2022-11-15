using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BAP.Db.Migrations
{
    public partial class GameStorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GamePlayLog",
                table: "GamePlayLog");

            migrationBuilder.RenameTable(
                name: "GamePlayLog",
                newName: "GamePlayLogs");

            migrationBuilder.RenameColumn(
                name: "ScoreInfoJson",
                table: "Scores",
                newName: "ScoringModelVersion");

            migrationBuilder.RenameColumn(
                name: "RelateiveScore",
                table: "Scores",
                newName: "NormalizedScore");

            migrationBuilder.AddColumn<int>(
                name: "ButtonCount",
                table: "Scores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ScoreData",
                table: "Scores",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GamePlayLogs",
                table: "GamePlayLogs",
                column: "GamePlayLogId");

            migrationBuilder.CreateTable(
                name: "ButtonLayouts",
                columns: table => new
                {
                    ButtonLayoutId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    ColumnCount = table.Column<int>(type: "int", nullable: false),
                    TotalButtons = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ButtonLayouts", x => x.ButtonLayoutId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GameStorageVault",
                columns: table => new
                {
                    GameStorageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameUniqueId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Data = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStorageVault", x => x.GameStorageId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ButtonPositions",
                columns: table => new
                {
                    ButtonPositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ButtonLayoutId = table.Column<int>(type: "int", nullable: false),
                    ButtonId = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RowId = table.Column<int>(type: "int", nullable: false),
                    ColumnId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ButtonPositions", x => x.ButtonPositionId);
                    table.ForeignKey(
                        name: "FK_ButtonPositions_ButtonLayouts_ButtonLayoutId",
                        column: x => x.ButtonLayoutId,
                        principalTable: "ButtonLayouts",
                        principalColumn: "ButtonLayoutId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ButtonPositions_ButtonLayoutId",
                table: "ButtonPositions",
                column: "ButtonLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_GameStorageVault_GameUniqueId",
                table: "GameStorageVault",
                column: "GameUniqueId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ButtonPositions");

            migrationBuilder.DropTable(
                name: "GameStorageVault");

            migrationBuilder.DropTable(
                name: "ButtonLayouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GamePlayLogs",
                table: "GamePlayLogs");

            migrationBuilder.DropColumn(
                name: "ButtonCount",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "ScoreData",
                table: "Scores");

            migrationBuilder.RenameTable(
                name: "GamePlayLogs",
                newName: "GamePlayLog");

            migrationBuilder.RenameColumn(
                name: "ScoringModelVersion",
                table: "Scores",
                newName: "ScoreInfoJson");

            migrationBuilder.RenameColumn(
                name: "NormalizedScore",
                table: "Scores",
                newName: "RelateiveScore");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GamePlayLog",
                table: "GamePlayLog",
                column: "GamePlayLogId");
        }
    }
}
