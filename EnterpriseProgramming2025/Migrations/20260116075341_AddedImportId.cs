using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseProgramming2025.Migrations
{
    /// <inheritdoc />
    public partial class AddedImportId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImportId",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportId",
                table: "MenuItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportId",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ImportId",
                table: "MenuItems");
        }
    }
}
