using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations.Project
{
    public partial class remove_filed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "company",
                table: "Projects");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "company",
                table: "Projects",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
