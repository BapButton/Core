using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BapDb.Migrations
{
    public partial class ButtonLayoutHistories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ButtonLayoutHistories",
                columns: table => new
                {
                    ButtonLayoutHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ButtonLayoutId = table.Column<int>(type: "int", nullable: false),
                    DateUsed = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ButtonLayoutHistories", x => x.ButtonLayoutHistoryId);
                    table.ForeignKey(
                        name: "FK_ButtonLayoutHistories_ButtonLayouts_ButtonLayoutId",
                        column: x => x.ButtonLayoutId,
                        principalTable: "ButtonLayouts",
                        principalColumn: "ButtonLayoutId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ButtonLayoutHistories_ButtonLayoutId",
                table: "ButtonLayoutHistories",
                column: "ButtonLayoutId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ButtonLayoutHistories");
        }
    }
}
