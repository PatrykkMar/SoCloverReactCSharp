using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoClover.Server.Migrations
{
    /// <inheritdoc />
    public partial class created_and_modified_at_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Players",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Players",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "GameRooms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Cards",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Cards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BoardSlots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "BoardSlots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Boards",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Boards",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BoardSlots");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "BoardSlots");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Boards");
        }
    }
}
