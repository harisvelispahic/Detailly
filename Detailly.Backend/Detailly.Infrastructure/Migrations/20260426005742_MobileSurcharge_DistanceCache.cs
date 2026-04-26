using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Detailly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MobileSurcharge_DistanceCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MobileSurchargeFee",
                table: "Bookings",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DistanceFromShopKm",
                table: "Addresses",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistanceFromShopLocationId",
                table: "Addresses",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobileSurchargeFee",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DistanceFromShopKm",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "DistanceFromShopLocationId",
                table: "Addresses");
        }
    }
}
