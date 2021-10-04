using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations
{
    public partial class update_taskItem_acl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "taskItem",
                table: "TimeTables");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "TaskItemAcls",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "timeTableId",
                table: "TaskItemAcls",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "TaskItemAcls");

            migrationBuilder.DropColumn(
                name: "timeTableId",
                table: "TaskItemAcls");

            migrationBuilder.AddColumn<long>(
                name: "taskItem",
                table: "TimeTables",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
