using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HM_byDH.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialWeightToWeightGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "InitialWeight",
                table: "WeightGoals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialWeight",
                table: "WeightGoals");
        }
    }
}
