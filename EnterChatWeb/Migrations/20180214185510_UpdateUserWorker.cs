using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EnterChatWeb.Migrations
{
    public partial class UpdateUserWorker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminEmail",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "AdminLogin",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "AdminName",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "AdminPassword",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "AdminSecondName",
                table: "Company");

            migrationBuilder.AlterColumn<string>(
                name: "SecondName",
                table: "Worker",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Worker",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "User",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "User",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WorkEmail",
                table: "Company",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Company",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SecondName",
                table: "Worker",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Worker",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "User",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "User",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "WorkEmail",
                table: "Company",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Company",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "AdminEmail",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminLogin",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminName",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminPassword",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminSecondName",
                table: "Company",
                nullable: true);
        }
    }
}
