using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations.Company
{
    public partial class fieldAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "Companies",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "Companies");

            migrationBuilder.AddColumn<long>(
                name: "user",
                table: "Companies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
