using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Detailly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _21022026_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingAddons");

            migrationBuilder.DropTable(
                name: "ServiceAddons");

            migrationBuilder.CreateTable(
                name: "BookingItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    ServicePackageItemId = table.Column<int>(type: "int", nullable: false),
                    PriceSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DurationMinutesSnapshot = table.Column<int>(type: "int", nullable: false),
                    RequiredEmployeesSnapshot = table.Column<int>(type: "int", nullable: false),
                    IsAddon = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ServicePackageItemEntityId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingItems_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingItems_ServicePackageItems_ServicePackageItemEntityId",
                        column: x => x.ServicePackageItemEntityId,
                        principalTable: "ServicePackageItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BookingItems_ServicePackageItems_ServicePackageItemId",
                        column: x => x.ServicePackageItemId,
                        principalTable: "ServicePackageItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_BookingId_ServicePackageItemId",
                table: "BookingItems",
                columns: new[] { "BookingId", "ServicePackageItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_ServicePackageItemEntityId",
                table: "BookingItems",
                column: "ServicePackageItemEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_ServicePackageItemId",
                table: "BookingItems",
                column: "ServicePackageItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingItems");

            migrationBuilder.CreateTable(
                name: "ServiceAddons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationMinutesDelta = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PriceDelta = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RequiredEmployeesDelta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAddons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingAddons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    ServiceAddonId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutesDeltaSnapshot = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PriceDeltaSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RequiredEmployeesDeltaSnapshot = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingAddons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingAddons_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingAddons_ServiceAddons_ServiceAddonId",
                        column: x => x.ServiceAddonId,
                        principalTable: "ServiceAddons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingAddons_BookingId_ServiceAddonId",
                table: "BookingAddons",
                columns: new[] { "BookingId", "ServiceAddonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingAddons_ServiceAddonId",
                table: "BookingAddons",
                column: "ServiceAddonId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAddons_IsActive",
                table: "ServiceAddons",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAddons_Name",
                table: "ServiceAddons",
                column: "Name");
        }
    }
}
