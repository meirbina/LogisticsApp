using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Migrations
{
    /// <inheritdoc />
    public partial class AddShipmentModificationAndCancellation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancellationDate",
                table: "Shipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Shipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                table: "Shipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Shipments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsModified",
                table: "Shipments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "Shipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModificationReason",
                table: "Shipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Shipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancellationDate",
                table: "MerchantShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "MerchantShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                table: "MerchantShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "MerchantShipments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsModified",
                table: "MerchantShipments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "MerchantShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModificationReason",
                table: "MerchantShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "MerchantShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancellationDate",
                table: "GenericShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "GenericShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                table: "GenericShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "GenericShipments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsModified",
                table: "GenericShipments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "GenericShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModificationReason",
                table: "GenericShipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "GenericShipments",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationDate",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "IsModified",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ModificationReason",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CancellationDate",
                table: "MerchantShipments");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "MerchantShipments");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "MerchantShipments");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "MerchantShipments");

            migrationBuilder.DropColumn(
                name: "IsModified",
                table: "MerchantShipments");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "MerchantShipments");

            migrationBuilder.DropColumn(
                name: "ModificationReason",
                table: "MerchantShipments");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "MerchantShipments");

            migrationBuilder.DropColumn(
                name: "CancellationDate",
                table: "GenericShipments");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "GenericShipments");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "GenericShipments");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "GenericShipments");

            migrationBuilder.DropColumn(
                name: "IsModified",
                table: "GenericShipments");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "GenericShipments");

            migrationBuilder.DropColumn(
                name: "ModificationReason",
                table: "GenericShipments");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "GenericShipments");
        }
    }
}
