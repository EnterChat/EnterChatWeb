﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EnterChatWeb.Migrations
{
    public partial class CompanyAdminUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "User",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "User",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
