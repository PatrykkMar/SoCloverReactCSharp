using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoClover.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemovingPlayerConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_ConnectionId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "Players");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "Players",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_ConnectionId",
                table: "Players",
                column: "ConnectionId");
        }
    }
}
