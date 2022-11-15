using Microsoft.EntityFrameworkCore.Migrations;

namespace BAP.Db.Migrations
{
    public partial class movingToDifficulty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScoringFactors",
                table: "Scores",
                newName: "DifficultyDetails");

            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "Scores",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Scores");

            migrationBuilder.RenameColumn(
                name: "DifficultyDetails",
                table: "Scores",
                newName: "ScoringFactors");
        }
    }
}
