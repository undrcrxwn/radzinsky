using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Radzinsky.Persistence.Migrations
{
    public partial class AddDefaultChatRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DefaultRoleId",
                table: "Chats",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ChatId1",
                table: "ChatPortals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Permissions = table.Column<string>(type: "text", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatMembers",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsChatAdministrator = table.Column<bool>(type: "boolean", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMembers", x => new { x.ChatId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ChatMembers_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMembers_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_DefaultRoleId",
                table: "Chats",
                column: "DefaultRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatPortals_ChatId1",
                table: "ChatPortals",
                column: "ChatId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMembers_RoleId",
                table: "ChatMembers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMembers_UserId",
                table: "ChatMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_ChatId",
                table: "Roles",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatPortals_Chats_ChatId1",
                table: "ChatPortals",
                column: "ChatId1",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Roles_DefaultRoleId",
                table: "Chats",
                column: "DefaultRoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatPortals_Chats_ChatId1",
                table: "ChatPortals");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Roles_DefaultRoleId",
                table: "Chats");

            migrationBuilder.DropTable(
                name: "ChatMembers");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Chats_DefaultRoleId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_ChatPortals_ChatId1",
                table: "ChatPortals");

            migrationBuilder.DropColumn(
                name: "DefaultRoleId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ChatId1",
                table: "ChatPortals");
        }
    }
}
