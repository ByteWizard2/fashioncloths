using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commercePro.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImageUrlsType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Products",
                newName: "stock");

            migrationBuilder.RenameColumn(
                name: "IsListed",
                table: "Products",
                newName: "islisted");

            migrationBuilder.RenameColumn(
                name: "Color",
                table: "Products",
                newName: "color");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "stock",
                table: "Products",
                newName: "Stock");

            migrationBuilder.RenameColumn(
                name: "islisted",
                table: "Products",
                newName: "IsListed");

            migrationBuilder.RenameColumn(
                name: "color",
                table: "Products",
                newName: "Color");
        }
    }
}
