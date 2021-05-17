using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.Migrations
{
    public partial class DevelopersSlugNameUniqueness : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("IX_Developers_Name", "Developers");
            migrationBuilder.DropIndex("IX_Developers_Slug", "Developers");

            migrationBuilder.CreateIndex(
                name: "IX_Developers_Name_Slug",
                table: "Developers",
                columns: new[] {"Slug", "Name"},
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("IX_Developers_Name_Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Developers_Name",
                table: "Developers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Developers_Slug",
                table: "Developers",
                column: "Slug",
                unique: true);
        }
    }
}
