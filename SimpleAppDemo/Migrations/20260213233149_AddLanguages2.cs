using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleAppDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguages2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "RamshaLanguages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "RamshaLanguages");
        }
    }
}
