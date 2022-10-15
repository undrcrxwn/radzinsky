using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Radzinsky.Persistence.Migrations
{
    public partial class AddSurveyState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "States",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MatrixCellId",
                table: "States",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "States",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "States");

            migrationBuilder.DropColumn(
                name: "MatrixCellId",
                table: "States");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "States");
        }
    }
}
