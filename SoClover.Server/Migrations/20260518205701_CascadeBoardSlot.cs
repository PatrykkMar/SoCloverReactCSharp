using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoClover.Server.Migrations
{
    /// <inheritdoc />
    public partial class CascadeBoardSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardSlots_Boards_BoardId",
                table: "BoardSlots");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardSlots_Boards_BoardId",
                table: "BoardSlots",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardSlots_Boards_BoardId",
                table: "BoardSlots");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardSlots_Boards_BoardId",
                table: "BoardSlots",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
