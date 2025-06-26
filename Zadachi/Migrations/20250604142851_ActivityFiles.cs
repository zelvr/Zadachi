using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zadachi.Migrations
{
    /// <inheritdoc />
    public partial class ActivityFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityFile",
                table: "Activities",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityFile",
                table: "Activities");
        }
    }
}
