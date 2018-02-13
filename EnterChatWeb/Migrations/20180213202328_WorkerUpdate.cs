using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EnterChatWeb.Migrations
{
    public partial class WorkerUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "SecondName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "User");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "User",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "WorkerID",
                table: "User",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Worker",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyID = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    InviteCode = table.Column<int>(nullable: false),
                    SecondName = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Worker_Company_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Company",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_WorkerID",
                table: "User",
                column: "WorkerID");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_CompanyID",
                table: "Worker",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Worker_WorkerID",
                table: "User",
                column: "WorkerID",
                principalTable: "Worker",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Worker_WorkerID",
                table: "User");

            migrationBuilder.DropTable(
                name: "Worker");

            migrationBuilder.DropIndex(
                name: "IX_User_WorkerID",
                table: "User");

            migrationBuilder.DropColumn(
                name: "WorkerID",
                table: "User");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "User",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondName",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "User",
                nullable: true);
        }
    }
}
