using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Migrations
{
    /// <inheritdoc />
    public partial class AddDevelopersPublishersMobyGamesId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "MobyGamesCompanyId",
                table: "Publishers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "MogyGamesCompanyId",
                table: "Developers",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobyGamesCompanyId",
                table: "Publishers");

            migrationBuilder.DropColumn(
                name: "MogyGamesCompanyId",
                table: "Developers");
        }
    }
}
