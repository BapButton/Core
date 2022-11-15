using Microsoft.EntityFrameworkCore.Migrations;

namespace BAP.Db.Migrations
{
    public partial class JustUseingScoringFactors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ButtonCount",
                table: "Scores");

            migrationBuilder.RenameColumn(
                name: "OtherScoringFactors",
                table: "Scores",
                newName: "ScoringFactors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScoringFactors",
                table: "Scores",
                newName: "OtherScoringFactors");

            migrationBuilder.AddColumn<int>(
                name: "ButtonCount",
                table: "Scores",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
