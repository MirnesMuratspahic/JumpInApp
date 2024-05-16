using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PickMeUpApp.Migrations
{
    /// <inheritdoc />
    public partial class Routetype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Routes");
        }
    }
}
