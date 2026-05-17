using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoClover.Server.Migrations
{
    /// <inheritdoc />
    public partial class TargetGameRoomAndRotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionIndex",
                table: "GameRoomCards");

            migrationBuilder.AddColumn<int>(
                name: "GameRoomCardId1",
                table: "BoardSlots",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PositionIndex",
                table: "BoardSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TargetGameRoomCardId",
                table: "BoardSlots",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TargetRotation",
                table: "BoardSlots",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoardSlots_GameRoomCardId1",
                table: "BoardSlots",
                column: "GameRoomCardId1",
                unique: true,
                filter: "[GameRoomCardId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BoardSlots_TargetGameRoomCardId",
                table: "BoardSlots",
                column: "TargetGameRoomCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardSlots_GameRoomCards_GameRoomCardId1",
                table: "BoardSlots",
                column: "GameRoomCardId1",
                principalTable: "GameRoomCards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardSlots_GameRoomCards_TargetGameRoomCardId",
                table: "BoardSlots",
                column: "TargetGameRoomCardId",
                principalTable: "GameRoomCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardSlots_GameRoomCards_GameRoomCardId1",
                table: "BoardSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_BoardSlots_GameRoomCards_TargetGameRoomCardId",
                table: "BoardSlots");

            migrationBuilder.DropIndex(
                name: "IX_BoardSlots_GameRoomCardId1",
                table: "BoardSlots");

            migrationBuilder.DropIndex(
                name: "IX_BoardSlots_TargetGameRoomCardId",
                table: "BoardSlots");

            migrationBuilder.DropColumn(
                name: "GameRoomCardId1",
                table: "BoardSlots");

            migrationBuilder.DropColumn(
                name: "PositionIndex",
                table: "BoardSlots");

            migrationBuilder.DropColumn(
                name: "TargetGameRoomCardId",
                table: "BoardSlots");

            migrationBuilder.DropColumn(
                name: "TargetRotation",
                table: "BoardSlots");

            migrationBuilder.AddColumn<int>(
                name: "PositionIndex",
                table: "GameRoomCards",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
