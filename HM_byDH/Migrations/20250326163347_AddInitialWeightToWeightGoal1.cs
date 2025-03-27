using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HM_byDH.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialWeightToWeightGoal1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DailyCaloriesTarget",
                table: "WeightGoals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyCaloriesTarget",
                table: "WeightGoals");
        }
    }
}
