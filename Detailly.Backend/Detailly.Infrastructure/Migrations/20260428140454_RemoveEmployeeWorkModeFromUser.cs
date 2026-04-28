using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Detailly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmployeeWorkModeFromUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_IsEmployee_EmployeeWorkMode_IsEnabled",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "EmployeeWorkMode",
                table: "ApplicationUsers");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_IsEmployee_IsEnabled",
                table: "ApplicationUsers",
                columns: new[] { "IsEmployee", "IsEnabled" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_IsEmployee_IsEnabled",
                table: "ApplicationUsers");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeWorkMode",
                table: "ApplicationUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_IsEmployee_EmployeeWorkMode_IsEnabled",
                table: "ApplicationUsers",
                columns: new[] { "IsEmployee", "EmployeeWorkMode", "IsEnabled" });
        }
    }
}
