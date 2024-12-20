using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Tickets.Migrations
{
    /// <inheritdoc />
    public partial class CinemaRequestModel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "CinemaRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "CinemaRequests");
        }
    }
}
