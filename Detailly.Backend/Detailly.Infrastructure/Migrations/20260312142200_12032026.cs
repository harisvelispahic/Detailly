using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Detailly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _12032026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedProducts_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedProducts_ApplicationUserId_ProductId",
                table: "SavedProducts",
                columns: new[] { "ApplicationUserId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedProducts_ProductId",
                table: "SavedProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedProducts");
        }
    }
}
