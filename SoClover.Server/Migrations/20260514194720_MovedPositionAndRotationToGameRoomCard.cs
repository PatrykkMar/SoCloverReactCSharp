using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoClover.Server.Migrations
{
    /// <inheritdoc />
    public partial class MovedPositionAndRotationToGameRoomCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentRotation",
                table: "BoardSlots");

            migrationBuilder.DropColumn(
                name: "PositionIndex",
                table: "BoardSlots");

            migrationBuilder.AddColumn<int>(
                name: "CurrentRotation",
                table: "GameRoomCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PositionIndex",
                table: "GameRoomCards",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentRotation",
                table: "GameRoomCards");

            migrationBuilder.DropColumn(
                name: "PositionIndex",
                table: "GameRoomCards");

            migrationBuilder.AddColumn<int>(
                name: "CurrentRotation",
                table: "BoardSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PositionIndex",
                table: "BoardSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
