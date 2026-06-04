using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wirtualny_Kibic.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrainingPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Intensity",
                table: "TrainingPlans",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "TrainingPlans",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Intensity",
                table: "TrainingPlans");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TrainingPlans");
        }
    }
}
