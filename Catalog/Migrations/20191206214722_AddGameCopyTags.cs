using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.Migrations
{
    public partial class AddGameCopyTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    color_argb = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')"),
                    LastUpdated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "GameCopyTag",
                columns: table => new
                {
                    GameCopyId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')"),
                    LastUpdated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameCopyTag", x => new { x.GameCopyId, x.TagId });
                    table.ForeignKey(
                        name: "FK_GameCopyTag_Games_GameCopyId",
                        column: x => x.GameCopyId,
                        principalTable: "Games",
                        principalColumn: "GameCopyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameCopyTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameCopyTag_TagId",
                table: "GameCopyTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameCopyTag");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
