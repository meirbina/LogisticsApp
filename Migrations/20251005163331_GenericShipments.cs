using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Migrations
{
    /// <inheritdoc />
    public partial class GenericShipments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GenericPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    RegularPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MerchantPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenericShipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WaybillNumber = table.Column<string>(type: "TEXT", nullable: true),
                    ShipmentType = table.Column<string>(type: "TEXT", nullable: true),
                    MerchantId = table.Column<int>(type: "INTEGER", nullable: true),
                    SenderName = table.Column<string>(type: "TEXT", nullable: true),
                    SenderPhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    SenderAddress = table.Column<string>(type: "TEXT", nullable: true),
                    ReceiverName = table.Column<string>(type: "TEXT", nullable: true),
                    ReceiverPhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    ReceiverAddress = table.Column<string>(type: "TEXT", nullable: true),
                    DestinationLocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    IsCollected = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShipmentCollectionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericShipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericShipments_Locations_DestinationLocationId",
                        column: x => x.DestinationLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenericShipments_Merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GenericShipments_ShipmentCollections_ShipmentCollectionId",
                        column: x => x.ShipmentCollectionId,
                        principalTable: "ShipmentCollections",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GenericShipmentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GenericShipmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    GenericPackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericShipmentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericShipmentItems_GenericPackages_GenericPackageId",
                        column: x => x.GenericPackageId,
                        principalTable: "GenericPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenericShipmentItems_GenericShipments_GenericShipmentId",
                        column: x => x.GenericShipmentId,
                        principalTable: "GenericShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenericShipmentItems_GenericPackageId",
                table: "GenericShipmentItems",
                column: "GenericPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericShipmentItems_GenericShipmentId",
                table: "GenericShipmentItems",
                column: "GenericShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericShipments_DestinationLocationId",
                table: "GenericShipments",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericShipments_MerchantId",
                table: "GenericShipments",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericShipments_ShipmentCollectionId",
                table: "GenericShipments",
                column: "ShipmentCollectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenericShipmentItems");

            migrationBuilder.DropTable(
                name: "GenericPackages");

            migrationBuilder.DropTable(
                name: "GenericShipments");
        }
    }
}
