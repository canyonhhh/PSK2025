using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSK2025.Data.Migrations
{
    /// <inheritdoc />
    public partial class pickup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Carts");

            migrationBuilder.AddColumn<DateTime>(
                name: "PickupTime",
                table: "Carts",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupTime",
                table: "Carts");

            migrationBuilder.AddColumn<int>(
                name: "TotalAmount",
                table: "Carts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
