using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Detailly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAddressTravelCacheColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistanceFromShopKm",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "TravelMetadataLocationId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "TravelTimeFromShopMinutes",
                table: "Addresses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DistanceFromShopKm",
                table: "Addresses",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TravelMetadataLocationId",
                table: "Addresses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TravelTimeFromShopMinutes",
                table: "Addresses",
                type: "int",
                nullable: true);
        }
    }
}
