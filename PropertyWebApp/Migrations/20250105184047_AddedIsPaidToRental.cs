using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsPaidToRental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isPaid",
                table: "MonthlyPayments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isPaid",
                table: "MonthlyPayments");
        }
    }
}
