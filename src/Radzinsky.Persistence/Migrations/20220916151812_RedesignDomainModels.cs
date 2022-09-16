using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Radzinsky.Persistence.Migrations
{
    public partial class RedesignDomainModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");
        }
    }
}
