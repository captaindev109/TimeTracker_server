using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations.Team
{
    public partial class add_team_lead : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "teamLead",
                table: "Teams",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "teamLead",
                table: "Teams");
        }
    }
}
