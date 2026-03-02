using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Detailly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _02032026_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_ApplicationUsers_ApplicationUserId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleCategories_VehicleCategoryId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_ApplicationUserId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_BookingVehicleAssignments_BookingId",
                table: "BookingVehicleAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "LicencePlate",
                table: "Vehicles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ApplicationUserId_LicencePlate",
                table: "Vehicles",
                columns: new[] { "ApplicationUserId", "LicencePlate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingVehicleAssignments_BookingId_VehicleId",
                table: "BookingVehicleAssignments",
                columns: new[] { "BookingId", "VehicleId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_ApplicationUsers_ApplicationUserId",
                table: "Vehicles",
                column: "ApplicationUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleCategories_VehicleCategoryId",
                table: "Vehicles",
                column: "VehicleCategoryId",
                principalTable: "VehicleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_ApplicationUsers_ApplicationUserId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleCategories_VehicleCategoryId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_ApplicationUserId_LicencePlate",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_BookingVehicleAssignments_BookingId_VehicleId",
                table: "BookingVehicleAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "LicencePlate",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ApplicationUserId",
                table: "Vehicles",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingVehicleAssignments_BookingId",
                table: "BookingVehicleAssignments",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_ApplicationUsers_ApplicationUserId",
                table: "Vehicles",
                column: "ApplicationUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleCategories_VehicleCategoryId",
                table: "Vehicles",
                column: "VehicleCategoryId",
                principalTable: "VehicleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
