using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Remapper.Test.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentItemDTOs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Timestamp = table.Column<DateTime>(maxLength: 200, nullable: false),
                    Author = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentItemDTOs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogPostDTOs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    Content = table.Column<string>(maxLength: 65000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostDTOs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPostDTOs_ContentItemDTOs_Id",
                        column: x => x.Id,
                        principalTable: "ContentItemDTOs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductReviewDTOs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Rating = table.Column<int>(nullable: false),
                    Review = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReviewDTOs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductReviewDTOs_ContentItemDTOs_Id",
                        column: x => x.Id,
                        principalTable: "ContentItemDTOs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPostDTOs");

            migrationBuilder.DropTable(
                name: "ProductReviewDTOs");

            migrationBuilder.DropTable(
                name: "ContentItemDTOs");
        }
    }
}
