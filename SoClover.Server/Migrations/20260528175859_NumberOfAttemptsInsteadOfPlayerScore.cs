using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoClover.Server.Migrations
{
    /// <inheritdoc />
    public partial class NumberOfAttemptsInsteadOfPlayerScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfAttempts",
                table: "GameRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfAttempts",
                table: "GameRooms");

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
