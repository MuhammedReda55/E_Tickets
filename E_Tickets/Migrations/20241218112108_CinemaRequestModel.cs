using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Tickets.Migrations
{
    /// <inheritdoc />
    public partial class CinemaRequestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CinemaRequestId",
                table: "Movies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CinemaRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CinemaLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CinemaRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CinemaRequestId",
                table: "Movies",
                column: "CinemaRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_CinemaRequests_CinemaRequestId",
                table: "Movies",
                column: "CinemaRequestId",
                principalTable: "CinemaRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_CinemaRequests_CinemaRequestId",
                table: "Movies");

            migrationBuilder.DropTable(
                name: "CinemaRequests");

            migrationBuilder.DropIndex(
                name: "IX_Movies_CinemaRequestId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "CinemaRequestId",
                table: "Movies");
        }
    }
}
