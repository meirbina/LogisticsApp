using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Migrations
{
    /// <inheritdoc />
    public partial class newManifestToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DispatchFee",
                table: "Manifests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DriverId",
                table: "Manifests",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Manifests_DepartureLocationId",
                table: "Manifests",
                column: "DepartureLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Manifests_DestinationLocationId",
                table: "Manifests",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Manifests_DriverId",
                table: "Manifests",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Manifests_Employees_DriverId",
                table: "Manifests",
                column: "DriverId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Manifests_Locations_DepartureLocationId",
                table: "Manifests",
                column: "DepartureLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Manifests_Locations_DestinationLocationId",
                table: "Manifests",
                column: "DestinationLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Manifests_Employees_DriverId",
                table: "Manifests");

            migrationBuilder.DropForeignKey(
                name: "FK_Manifests_Locations_DepartureLocationId",
                table: "Manifests");

            migrationBuilder.DropForeignKey(
                name: "FK_Manifests_Locations_DestinationLocationId",
                table: "Manifests");

            migrationBuilder.DropIndex(
                name: "IX_Manifests_DepartureLocationId",
                table: "Manifests");

            migrationBuilder.DropIndex(
                name: "IX_Manifests_DestinationLocationId",
                table: "Manifests");

            migrationBuilder.DropIndex(
                name: "IX_Manifests_DriverId",
                table: "Manifests");

            migrationBuilder.DropColumn(
                name: "DispatchFee",
                table: "Manifests");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Manifests");
        }
    }
}
