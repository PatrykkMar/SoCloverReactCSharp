using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoClover.Server.Migrations
{
    /// <inheritdoc />
    public partial class RenamedActivePlayerToCheckedPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameRooms_Players_ActivePlayerId",
                table: "GameRooms");

            migrationBuilder.RenameColumn(
                name: "ActivePlayerId",
                table: "GameRooms",
                newName: "CheckedPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_GameRooms_ActivePlayerId",
                table: "GameRooms",
                newName: "IX_GameRooms_CheckedPlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameRooms_Players_CheckedPlayerId",
                table: "GameRooms",
                column: "CheckedPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameRooms_Players_CheckedPlayerId",
                table: "GameRooms");

            migrationBuilder.RenameColumn(
                name: "CheckedPlayerId",
                table: "GameRooms",
                newName: "ActivePlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_GameRooms_CheckedPlayerId",
                table: "GameRooms",
                newName: "IX_GameRooms_ActivePlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameRooms_Players_ActivePlayerId",
                table: "GameRooms",
                column: "ActivePlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
