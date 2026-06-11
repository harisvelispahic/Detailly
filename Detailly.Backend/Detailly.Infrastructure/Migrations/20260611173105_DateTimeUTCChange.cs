using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Detailly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DateTimeUTCChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OpenTimeUtc",
                table: "LocationOpeningHours",
                newName: "OpenTime");

            migrationBuilder.RenameColumn(
                name: "CloseTimeUtc",
                table: "LocationOpeningHours",
                newName: "CloseTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OpenTime",
                table: "LocationOpeningHours",
                newName: "OpenTimeUtc");

            migrationBuilder.RenameColumn(
                name: "CloseTime",
                table: "LocationOpeningHours",
                newName: "CloseTimeUtc");
        }
    }
}
