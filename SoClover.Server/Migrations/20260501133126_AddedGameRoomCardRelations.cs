using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoClover.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedGameRoomCardRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardSlots_Cards_CardId",
                table: "BoardSlots");

            migrationBuilder.DropIndex(
                name: "IX_BoardSlots_CardId",
                table: "BoardSlots");

            migrationBuilder.DropColumn(
                name: "CardId",
                table: "BoardSlots");

            migrationBuilder.AddColumn<int>(
                name: "GameRoomCardId",
                table: "BoardSlots",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoardSlots_GameRoomCardId",
                table: "BoardSlots",
                column: "GameRoomCardId",
                unique: true,
                filter: "[GameRoomCardId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardSlots_GameRoomCards_GameRoomCardId",
                table: "BoardSlots",
                column: "GameRoomCardId",
                principalTable: "GameRoomCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardSlots_GameRoomCards_GameRoomCardId",
                table: "BoardSlots");

            migrationBuilder.DropIndex(
                name: "IX_BoardSlots_GameRoomCardId",
                table: "BoardSlots");

            migrationBuilder.DropColumn(
                name: "GameRoomCardId",
                table: "BoardSlots");

            migrationBuilder.AddColumn<int>(
                name: "CardId",
                table: "BoardSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BoardSlots_CardId",
                table: "BoardSlots",
                column: "CardId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardSlots_Cards_CardId",
                table: "BoardSlots",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
