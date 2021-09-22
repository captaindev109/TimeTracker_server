using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations
{
    public partial class update_field_taskItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "company",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "duration",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "end",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "project",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "start",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "tags",
                table: "TaskItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "company",
                table: "TaskItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "duration",
                table: "TaskItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "end",
                table: "TaskItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "project",
                table: "TaskItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "start",
                table: "TaskItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string[]>(
                name: "tags",
                table: "TaskItems",
                type: "text[]",
                nullable: true);
        }
    }
}
