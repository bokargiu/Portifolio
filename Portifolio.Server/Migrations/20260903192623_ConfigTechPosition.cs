using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portifolio.Server.Migrations
{
    /// <inheritdoc />
    public partial class ConfigTechPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Technologies_Position",
                table: "Technologies",
                column: "Position",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Technologies_Position",
                table: "Technologies");
        }
    }
}
