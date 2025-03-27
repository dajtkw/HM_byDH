using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HM_byDH.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIntesityActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CaloriesBurned",
                table: "ActivityEntries",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaloriesBurned",
                table: "ActivityEntries");
        }
    }
}
