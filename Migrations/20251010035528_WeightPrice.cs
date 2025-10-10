using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Migrations
{
    /// <inheritdoc />
    public partial class WeightPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndKg",
                table: "WeightPrices");

            migrationBuilder.RenameColumn(
                name: "StartKg",
                table: "WeightPrices",
                newName: "Weight");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "WeightPrices",
                newName: "StartKg");

            migrationBuilder.AddColumn<decimal>(
                name: "EndKg",
                table: "WeightPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
