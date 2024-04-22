using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commercePro.Migrations
{
    /// <inheritdoc />
    public partial class UserSignUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Sign_Up",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Sign_Up");
        }
    }
}
