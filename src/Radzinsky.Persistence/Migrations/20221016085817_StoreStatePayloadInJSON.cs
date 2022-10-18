using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Radzinsky.Persistence.Migrations
{
    public partial class StoreStatePayloadInJSON : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatrixCellId",
                table: "States");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "States");

            migrationBuilder.RenameColumn(
                name: "Discriminator",
                table: "States",
                newName: "Payload");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Payload",
                table: "States",
                newName: "Discriminator");

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
    }
}
