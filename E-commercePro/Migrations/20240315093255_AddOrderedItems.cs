using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commercePro.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderedItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedItem_Orders_OrderId",
                table: "OrderedItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedItem_Products_ProductId",
                table: "OrderedItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderedItem",
                table: "OrderedItem");

            migrationBuilder.RenameTable(
                name: "OrderedItem",
                newName: "OrderedItems");

            migrationBuilder.RenameIndex(
                name: "IX_OrderedItem_ProductId",
                table: "OrderedItems",
                newName: "IX_OrderedItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderedItem_OrderId",
                table: "OrderedItems",
                newName: "IX_OrderedItems_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderedItems",
                table: "OrderedItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedItems_Orders_OrderId",
                table: "OrderedItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedItems_Products_ProductId",
                table: "OrderedItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedItems_Orders_OrderId",
                table: "OrderedItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedItems_Products_ProductId",
                table: "OrderedItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderedItems",
                table: "OrderedItems");

            migrationBuilder.RenameTable(
                name: "OrderedItems",
                newName: "OrderedItem");

            migrationBuilder.RenameIndex(
                name: "IX_OrderedItems_ProductId",
                table: "OrderedItem",
                newName: "IX_OrderedItem_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderedItems_OrderId",
                table: "OrderedItem",
                newName: "IX_OrderedItem_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderedItem",
                table: "OrderedItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedItem_Orders_OrderId",
                table: "OrderedItem",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedItem_Products_ProductId",
                table: "OrderedItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
