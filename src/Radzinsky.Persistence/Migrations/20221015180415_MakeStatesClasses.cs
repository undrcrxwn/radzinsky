using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Radzinsky.Persistence.Migrations
{
    public partial class MakeStatesClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_States_State_StateId",
                table: "States");

            migrationBuilder.DropTable(
                name: "State");

            migrationBuilder.DropIndex(
                name: "IX_States_StateId",
                table: "States");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "States");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StateId",
                table: "States",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_States_StateId",
                table: "States",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_States_State_StateId",
                table: "States",
                column: "StateId",
                principalTable: "State",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
