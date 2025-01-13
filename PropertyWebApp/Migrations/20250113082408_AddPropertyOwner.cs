using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PropertyOwnerId",
                table: "Properties",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_PropertyOwnerId",
                table: "Properties",
                column: "PropertyOwnerId");

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

            migrationBuilder.DropIndex(
                name: "IX_Properties_PropertyOwnerId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PropertyOwnerId",
                table: "Properties");
        }
    }
}
