using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Migrations
{
    /// <inheritdoc />
    public partial class CorrectDeveloperMobyGamesCompanyIdTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MogyGamesCompanyId",
                table: "Developers",
                newName: "MobyGamesCompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MobyGamesCompanyId",
                table: "Developers",
                newName: "MogyGamesCompanyId");
        }
    }
}
