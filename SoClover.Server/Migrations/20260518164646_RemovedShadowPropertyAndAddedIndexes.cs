using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoClover.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemovedShadowPropertyAndAddedIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardSlots_Boards_BoardId",
                table: "BoardSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_BoardSlots_GameRoomCards_GameRoomCardId1",
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

            migrationBuilder.CreateIndex(
                name: "IX_Players_ConnectionId",
                table: "Players",
                column: "ConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlayerGuid",
                table: "Players",
                column: "PlayerGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameRooms_RoomCode",
                table: "GameRooms",
                column: "RoomCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoardSlots_TargetGameRoomCardId",
                table: "BoardSlots",
                column: "TargetGameRoomCardId",
                unique: true,
                filter: "[TargetGameRoomCardId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardSlots_Boards_BoardId",
                table: "BoardSlots",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardSlots_Boards_BoardId",
                table: "BoardSlots");

            migrationBuilder.DropIndex(
                name: "IX_Players_ConnectionId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_PlayerGuid",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_GameRooms_RoomCode",
                table: "GameRooms");

            migrationBuilder.DropIndex(
                name: "IX_BoardSlots_TargetGameRoomCardId",
                table: "BoardSlots");

            migrationBuilder.AddColumn<int>(
                name: "GameRoomCardId1",
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
                name: "FK_BoardSlots_Boards_BoardId",
                table: "BoardSlots",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BoardSlots_GameRoomCards_GameRoomCardId1",
                table: "BoardSlots",
                column: "GameRoomCardId1",
                principalTable: "GameRoomCards",
                principalColumn: "Id");
        }
    }
}
