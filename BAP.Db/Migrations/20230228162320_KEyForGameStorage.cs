using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BAP.Db.Migrations
{
    /// <inheritdoc />
    public partial class KEyForGameStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "GameStorageVault",
                type: "varchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "default")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "GameStorageVault");
        }
    }
}
