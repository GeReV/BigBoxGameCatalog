using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.Migrations
{
    public partial class DropDevelopersGameCopyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Developers_Games_GameCopyId",
                table: "Developers");

            migrationBuilder.DropIndex(
                name: "IX_Developers_GameCopyId",
                table: "Developers");

            migrationBuilder.DropColumn(
                name: "GameCopyId",
                table: "Developers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameCopyId",
                table: "Developers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Developers_GameCopyId",
                table: "Developers",
                column: "GameCopyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Developers_Games_GameCopyId",
                table: "Developers",
                column: "GameCopyId",
                principalTable: "Games",
                principalColumn: "GameCopyId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
