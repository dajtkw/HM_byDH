using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HM_byDH.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIntesityActivity1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Intensity",
                table: "ActivityEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Intensity",
                table: "ActivityEntries");
        }
    }
}
