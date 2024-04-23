using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PickMeUpApp.Migrations
{
    /// <inheritdoc />
    public partial class pricemigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "Routes",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Routes");
        }
    }
}
