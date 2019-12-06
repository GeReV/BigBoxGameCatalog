using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    PublisherId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Slug = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Links = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')"),
                    LastUpdated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.PublisherId);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameCopyId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: false),
                    Sealed = table.Column<bool>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    MobyGamesSlug = table.Column<string>(nullable: true),
                    PublisherId = table.Column<int>(nullable: true),
                    ReleaseDate = table.Column<DateTime>(nullable: false),
                    TwoLetterIsoLanguageName = table.Column<string>(nullable: true),
                    Platforms = table.Column<string>(nullable: true),
                    Links = table.Column<string>(nullable: true),
                    CoverImage = table.Column<string>(nullable: true),
                    Screenshots = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')"),
                    LastUpdated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameCopyId);
                    table.ForeignKey(
                        name: "FK_Games_Publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Publishers",
                        principalColumn: "PublisherId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Developers",
                columns: table => new
                {
                    DeveloperId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Slug = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Links = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')"),
                    LastUpdated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')"),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Developers", x => x.DeveloperId);
                });

            migrationBuilder.CreateTable(
                name: "GameItem",
                columns: table => new
                {
                    GameItemId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemType = table.Column<string>(nullable: false),
                    GameCopyId = table.Column<int>(nullable: false),
                    Missing = table.Column<bool>(nullable: false),
                    Condition = table.Column<int>(nullable: true),
                    ConditionDetails = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')"),
                    LastUpdated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameItem", x => x.GameItemId);
                    table.ForeignKey(
                        name: "FK_GameItem_Games_GameCopyId",
                        column: x => x.GameCopyId,
                        principalTable: "Games",
                        principalColumn: "GameCopyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameCopyDeveloper",
                columns: table => new
                {
                    GameCopyId = table.Column<int>(nullable: false),
                    DeveloperId = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')"),
                    LastUpdated = table.Column<DateTime>(nullable: false, defaultValueSql: "DATETIME('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameCopyDeveloper", x => new { x.GameCopyId, x.DeveloperId });
                    table.ForeignKey(
                        name: "FK_GameCopyDeveloper_Developers_DeveloperId",
                        column: x => x.DeveloperId,
                        principalTable: "Developers",
                        principalColumn: "DeveloperId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameCopyDeveloper_Games_GameCopyId",
                        column: x => x.GameCopyId,
                        principalTable: "Games",
                        principalColumn: "GameCopyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    FileId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameItemId = table.Column<int>(nullable: true),
                    Sha256Checksum = table.Column<byte[]>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.FileId);
                    table.ForeignKey(
                        name: "FK_File_GameItem_GameItemId",
                        column: x => x.GameItemId,
                        principalTable: "GameItem",
                        principalColumn: "GameItemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    ImageId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameItemId = table.Column<int>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_Image_GameItem_GameItemId",
                        column: x => x.GameItemId,
                        principalTable: "GameItem",
                        principalColumn: "GameItemId",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_File_GameItemId",
                table: "File",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GameCopyDeveloper_DeveloperId",
                table: "GameCopyDeveloper",
                column: "DeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_GameItem_GameCopyId",
                table: "GameItem",
                column: "GameCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_MobyGamesSlug",
                table: "Games",
                column: "MobyGamesSlug");

            migrationBuilder.CreateIndex(
                name: "IX_Games_PublisherId",
                table: "Games",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Title",
                table: "Games",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Image_GameItemId",
                table: "Image",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_Name",
                table: "Publishers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_Slug",
                table: "Publishers",
                column: "Slug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "GameCopyDeveloper");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "Developers");

            migrationBuilder.DropTable(
                name: "GameItem");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Publishers");
        }
    }
}
