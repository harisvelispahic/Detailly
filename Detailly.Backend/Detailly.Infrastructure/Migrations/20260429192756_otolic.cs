using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Detailly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class otolic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTemporarilyClosed",
                table: "Locations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTemporarilyClosed",
                table: "Locations");
        }
    }
}
