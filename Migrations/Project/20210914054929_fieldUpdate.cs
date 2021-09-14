using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations.Project
{
    public partial class fieldUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "projectManager",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "projectManagerAssistant",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "teams",
                table: "Projects");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long[]>(
                name: "projectManager",
                table: "Projects",
                type: "bigint[]",
                nullable: true);

            migrationBuilder.AddColumn<long[]>(
                name: "projectManagerAssistant",
                table: "Projects",
                type: "bigint[]",
                nullable: true);

            migrationBuilder.AddColumn<long[]>(
                name: "teams",
                table: "Projects",
                type: "bigint[]",
                nullable: true);
        }
    }
}
