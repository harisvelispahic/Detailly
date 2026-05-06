using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Detailly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorImageRelationships_AddCloudinary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_Images_ImageId",
                table: "ApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_ApplicationUsers_ApplicationUserId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Products_ProductEntityId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Reviews_ReviewId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_ServicePackageItems_ServicePackageItemId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ApplicationUserId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ProductEntityId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ReviewId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_ImageId",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ProductEntityId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "ApplicationUsers");

            migrationBuilder.RenameColumn(
                name: "ServicePackageItemId",
                table: "Images",
                newName: "ServicePackageId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_ServicePackageItemId",
                table: "Images",
                newName: "IX_Images_ServicePackageId");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ServicePackages_ServicePackageId",
                table: "Images",
                column: "ServicePackageId",
                principalTable: "ServicePackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_ServicePackages_ServicePackageId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "ServicePackageId",
                table: "Images",
                newName: "ServicePackageItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_ServicePackageId",
                table: "Images",
                newName: "IX_Images_ServicePackageItemId");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductEntityId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "ApplicationUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_ApplicationUserId",
                table: "Images",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ProductEntityId",
                table: "Images",
                column: "ProductEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ReviewId",
                table: "Images",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_ImageId",
                table: "ApplicationUsers",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_Images_ImageId",
                table: "ApplicationUsers",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ApplicationUsers_ApplicationUserId",
                table: "Images",
                column: "ApplicationUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Products_ProductEntityId",
                table: "Images",
                column: "ProductEntityId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Reviews_ReviewId",
                table: "Images",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ServicePackageItems_ServicePackageItemId",
                table: "Images",
                column: "ServicePackageItemId",
                principalTable: "ServicePackageItems",
                principalColumn: "Id");
        }
    }
}
