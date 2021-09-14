using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations.Team
{
    public partial class remove_filed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "company",
                table: "Teams");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "company",
                table: "Teams",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
