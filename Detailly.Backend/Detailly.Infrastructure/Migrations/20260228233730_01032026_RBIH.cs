using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Detailly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _01032026_RBIH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TimeSlots_TimeSlotEntityId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TimeSlotEntityId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TimeSlotEntityId",
                table: "Bookings");

            migrationBuilder.CreateTable(
                name: "LocationOpeningHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopLocationId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    OpenTimeUtc = table.Column<TimeSpan>(type: "time", nullable: true),
                    CloseTimeUtc = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationOpeningHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationOpeningHours_Locations_ShopLocationId",
                        column: x => x.ShopLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationOpeningHours_ShopLocationId",
                table: "LocationOpeningHours",
                column: "ShopLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationOpeningHours_ShopLocationId_DayOfWeek",
                table: "LocationOpeningHours",
                columns: new[] { "ShopLocationId", "DayOfWeek" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationOpeningHours");

            migrationBuilder.AddColumn<int>(
                name: "TimeSlotEntityId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSlots_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TimeSlotEntityId",
                table: "Bookings",
                column: "TimeSlotEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_LocationId",
                table: "TimeSlots",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TimeSlots_TimeSlotEntityId",
                table: "Bookings",
                column: "TimeSlotEntityId",
                principalTable: "TimeSlots",
                principalColumn: "Id");
        }
    }
}
