using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations.ResourceType
{
    public partial class UpdateField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tag",
                table: "ResourceTypes");

            migrationBuilder.AddColumn<List<string>>(
                name: "tags",
                table: "ResourceTypes",
                type: "text[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tags",
                table: "ResourceTypes");

            migrationBuilder.AddColumn<string>(
                name: "tag",
                table: "ResourceTypes",
                type: "text",
                nullable: true);
        }
    }
}
