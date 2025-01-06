using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthlyPaymentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentCostId",
                table: "MonthlyPayments");

            migrationBuilder.DropColumn(
                name: "UtilitiesCostId",
                table: "MonthlyPayments");

            migrationBuilder.AlterColumn<int>(
                name: "RentalId",
                table: "MonthlyPayments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "RentAmount",
                table: "MonthlyPayments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UtilitiesAmount",
                table: "MonthlyPayments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyPayments_RentalId",
                table: "MonthlyPayments",
                column: "RentalId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyPayments_Rentals_RentalId",
                table: "MonthlyPayments",
                column: "RentalId",
                principalTable: "Rentals",
                principalColumn: "RentalId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyPayments_Rentals_RentalId",
                table: "MonthlyPayments");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyPayments_RentalId",
                table: "MonthlyPayments");

            migrationBuilder.DropColumn(
                name: "RentAmount",
                table: "MonthlyPayments");

            migrationBuilder.DropColumn(
                name: "UtilitiesAmount",
                table: "MonthlyPayments");

            migrationBuilder.AlterColumn<string>(
                name: "RentalId",
                table: "MonthlyPayments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RentCostId",
                table: "MonthlyPayments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UtilitiesCostId",
                table: "MonthlyPayments",
                type: "int",
                nullable: true);
        }
    }
}
