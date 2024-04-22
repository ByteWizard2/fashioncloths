using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commercePro.Migrations
{
    /// <inheritdoc />
    public partial class AddWhishlistTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "whishlist",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_whishlist", x => x.id);
                    table.ForeignKey(
                        name: "FK_whishlist_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_whishlist_Sign_Up_UserId",
                        column: x => x.UserId,
                        principalTable: "Sign_Up",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_whishlist_ProductId",
                table: "whishlist",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_whishlist_UserId",
                table: "whishlist",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "whishlist");
        }
    }
}
