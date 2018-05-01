using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EnterChatWeb.Migrations
{
    public partial class NoteCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TopicMessage_User_UserID",
                table: "TopicMessage");

            migrationBuilder.DropIndex(
                name: "IX_TopicMessage_UserID",
                table: "TopicMessage");

            migrationBuilder.AddColumn<int>(
                name: "NoteCategoryID",
                table: "Note",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NoteCategory",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyID = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteCategory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NoteCategory_Company_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Company",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Note_NoteCategoryID",
                table: "Note",
                column: "NoteCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_NoteCategory_CompanyID",
                table: "NoteCategory",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_NoteCategory_NoteCategoryID",
                table: "Note",
                column: "NoteCategoryID",
                principalTable: "NoteCategory",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_NoteCategory_NoteCategoryID",
                table: "Note");

            migrationBuilder.DropTable(
                name: "NoteCategory");

            migrationBuilder.DropIndex(
                name: "IX_Note_NoteCategoryID",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "NoteCategoryID",
                table: "Note");

            migrationBuilder.CreateIndex(
                name: "IX_TopicMessage_UserID",
                table: "TopicMessage",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_TopicMessage_User_UserID",
                table: "TopicMessage",
                column: "UserID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
