using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDevelopersPublishersSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropIndex(
            //     name: "IX_Publishers_Slug",
            //     table: "Publishers");
            //
            // migrationBuilder.DropIndex(
            //     name: "IX_Developers_Slug",
            //     table: "Developers");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Publishers");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Developers");

            migrationBuilder.AlterColumn<uint>(
                name: "MobyGamesCompanyId",
                table: "Publishers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u,
                oldClrType: typeof(uint),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "MobyGamesCompanyId",
                table: "Developers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u,
                oldClrType: typeof(uint),
                oldType: "INTEGER",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<uint>(
                name: "MobyGamesCompanyId",
                table: "Publishers",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(uint),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Publishers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<uint>(
                name: "MobyGamesCompanyId",
                table: "Developers",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(uint),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Developers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_Slug",
                table: "Publishers",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Developers_Slug",
                table: "Developers",
                column: "Slug",
                unique: true);
        }
    }
}
