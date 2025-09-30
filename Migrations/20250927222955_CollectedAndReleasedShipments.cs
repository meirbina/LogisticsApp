using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Migrations
{
    /// <inheritdoc />
    public partial class CollectedAndReleasedShipments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCollected",
                table: "Shipments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReceivedBy",
                table: "Shipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedDate",
                table: "Shipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipmentCollectionId",
                table: "Shipments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCollected",
                table: "MerchantShipments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReceivedBy",
                table: "MerchantShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedDate",
                table: "MerchantShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipmentCollectionId",
                table: "MerchantShipments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShipmentCollections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WaybillNumber = table.Column<string>(type: "TEXT", nullable: false),
                    CollectedByName = table.Column<string>(type: "TEXT", nullable: false),
                    CollectedByPhone = table.Column<string>(type: "TEXT", nullable: false),
                    CollectedByAddress = table.Column<string>(type: "TEXT", nullable: true),
                    MeansOfId = table.Column<string>(type: "TEXT", nullable: true),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    CollectionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReleasedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentCollections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShipmentCollectionId",
                table: "Shipments",
                column: "ShipmentCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantShipments_ShipmentCollectionId",
                table: "MerchantShipments",
                column: "ShipmentCollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MerchantShipments_ShipmentCollections_ShipmentCollectionId",
                table: "MerchantShipments",
                column: "ShipmentCollectionId",
                principalTable: "ShipmentCollections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_ShipmentCollections_ShipmentCollectionId",
                table: "Shipments",
                column: "ShipmentCollectionId",
                principalTable: "ShipmentCollections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MerchantShipments_ShipmentCollections_ShipmentCollectionId",
                table: "MerchantShipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_ShipmentCollections_ShipmentCollectionId",
                table: "Shipments");

            migrationBuilder.DropTable(
                name: "ShipmentCollections");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_ShipmentCollectionId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_MerchantShipments_ShipmentCollectionId",
                table: "MerchantShipments");

            migrationBuilder.DropColumn(
                name: "IsCollected",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ReceivedDate",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ShipmentCollectionId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "IsCollected",
                table: "MerchantShipments");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "MerchantShipments");

            migrationBuilder.DropColumn(
                name: "ReceivedDate",
                table: "MerchantShipments");

            migrationBuilder.DropColumn(
                name: "ShipmentCollectionId",
                table: "MerchantShipments");
        }
    }
}
