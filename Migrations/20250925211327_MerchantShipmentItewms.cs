using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Migrations
{
    /// <inheritdoc />
    public partial class MerchantShipmentItewms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MerchantShipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WaybillNumber = table.Column<string>(type: "TEXT", nullable: false),
                    MerchantId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceiverName = table.Column<string>(type: "TEXT", nullable: false),
                    ReceiverPhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ReceiverAddress = table.Column<string>(type: "TEXT", nullable: false),
                    ReceiverStateId = table.Column<int>(type: "INTEGER", nullable: false),
                    DestinationLocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", nullable: true),
                    DeclaredValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    InsuranceId = table.Column<int>(type: "INTEGER", nullable: true),
                    ShipmentCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    PackagingCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    InsuranceCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    Vat = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantShipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchantShipments_Locations_DestinationLocationId",
                        column: x => x.DestinationLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MerchantShipments_Merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantShipmentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MerchantShipmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: false),
                    Condition = table.Column<string>(type: "TEXT", nullable: true),
                    PackagingPriceId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberOfPackagingItems = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantShipmentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchantShipmentItems_MerchantShipments_MerchantShipmentId",
                        column: x => x.MerchantShipmentId,
                        principalTable: "MerchantShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MerchantShipmentItems_PackagingPrices_PackagingPriceId",
                        column: x => x.PackagingPriceId,
                        principalTable: "PackagingPrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MerchantShipmentItems_MerchantShipmentId",
                table: "MerchantShipmentItems",
                column: "MerchantShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantShipmentItems_PackagingPriceId",
                table: "MerchantShipmentItems",
                column: "PackagingPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantShipments_DestinationLocationId",
                table: "MerchantShipments",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantShipments_MerchantId",
                table: "MerchantShipments",
                column: "MerchantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MerchantShipmentItems");

            migrationBuilder.DropTable(
                name: "MerchantShipments");
        }
    }
}
