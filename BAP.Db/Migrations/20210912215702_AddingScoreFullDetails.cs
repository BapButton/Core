using Microsoft.EntityFrameworkCore.Migrations;

namespace BAP.Db.Migrations
{
    public partial class AddingScoreFullDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScoreFullDetails",
                table: "Scores",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScoreFullDetails",
                table: "Scores");
        }
    }
}
