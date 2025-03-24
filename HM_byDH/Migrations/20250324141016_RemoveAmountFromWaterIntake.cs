using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HM_byDH.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAmountFromWaterIntake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "WaterIntakes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "WaterIntakes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
