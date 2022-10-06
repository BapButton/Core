using Microsoft.EntityFrameworkCore.Migrations;

namespace BapDb.Migrations
{
    public partial class ScoreUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherScoringFactors",
                table: "Scores",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherScoringFactors",
                table: "Scores");
        }
    }
}
