using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portifolio.Server.Migrations
{
    /// <inheritdoc />
    public partial class TechPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Technologies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Technologies");
        }
    }
}
