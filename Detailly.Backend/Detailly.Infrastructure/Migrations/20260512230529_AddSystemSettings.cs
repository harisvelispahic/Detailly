using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Detailly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StandardWalletBonusPercent = table.Column<int>(type: "int", nullable: false),
                    FleetWalletBonusPercent = table.Column<int>(type: "int", nullable: false),
                    ReviewWindowDays = table.Column<int>(type: "int", nullable: false),
                    BaseFleetDiscountPercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PerVehicleFleetDiscountPercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxFleetDiscountPercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "BaseFleetDiscountPercent", "CreatedAtUtc", "FleetWalletBonusPercent", "IsDeleted", "MaxFleetDiscountPercent", "ModifiedAtUtc", "PerVehicleFleetDiscountPercent", "ReviewWindowDays", "StandardWalletBonusPercent" },
                values: new object[] { 1, 2.0m, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), 15, false, 8.0m, null, 1.0m, 7, 10 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemSettings");
        }
    }
}
