using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations
{
    public partial class update_user_acl_field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "companyId",
                table: "UserAcls",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "companyId",
                table: "UserAcls");
        }
    }
}
