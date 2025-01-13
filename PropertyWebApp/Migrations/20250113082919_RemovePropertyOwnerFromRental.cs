using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class RemovePropertyOwnerFromRental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_AspNetUsers_PropertyOwnerId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PropertyOwnerId",
                table: "Rentals");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_AspNetUsers_PropertyOwnerId",
                table: "Properties",
                column: "PropertyOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_AspNetUsers_PropertyOwnerId",
                table: "Properties");

            migrationBuilder.AddColumn<string>(
                name: "PropertyOwnerId",
                table: "Rentals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_AspNetUsers_PropertyOwnerId",
                table: "Properties",
                column: "PropertyOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
