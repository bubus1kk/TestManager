using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTestDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tests",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tests");
        }
    }
}
