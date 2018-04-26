using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EnterChatWeb.Migrations
{
    public partial class ChatMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_File_User_UserID",
                table: "File");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatMessage_User_UserID",
                table: "GroupChatMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_Note_User_UserID",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_UserID",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_GroupChatMessage_UserID",
                table: "GroupChatMessage");

            migrationBuilder.DropIndex(
                name: "IX_File_UserID",
                table: "File");

            migrationBuilder.CreateTable(
                name: "ChatMembers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TopicID = table.Column<int>(type: "int", nullable: true),
                    WorkerID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMembers", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMembers");

            migrationBuilder.CreateIndex(
                name: "IX_Note_UserID",
                table: "Note",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatMessage_UserID",
                table: "GroupChatMessage",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_File_UserID",
                table: "File",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_File_User_UserID",
                table: "File",
                column: "UserID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatMessage_User_UserID",
                table: "GroupChatMessage",
                column: "UserID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Note_User_UserID",
                table: "Note",
                column: "UserID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
