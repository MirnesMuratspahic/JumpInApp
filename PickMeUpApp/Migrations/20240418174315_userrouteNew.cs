using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PickMeUpApp.Migrations
{
    /// <inheritdoc />
    public partial class userrouteNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserRouteId = table.Column<int>(type: "int", nullable: false),
                    PassengerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_UserRoutes_UserRouteId",
                        column: x => x.UserRouteId,
                        principalTable: "UserRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoutes_RouteId",
                table: "UserRoutes",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoutes_UserId",
                table: "UserRoutes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_UserRouteId",
                table: "Requests",
                column: "UserRouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoutes_Routes_RouteId",
                table: "UserRoutes",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoutes_Users_UserId",
                table: "UserRoutes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoutes_Routes_RouteId",
                table: "UserRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoutes_Users_UserId",
                table: "UserRoutes");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_UserRoutes_RouteId",
                table: "UserRoutes");

            migrationBuilder.DropIndex(
                name: "IX_UserRoutes_UserId",
                table: "UserRoutes");
        }
    }
}
