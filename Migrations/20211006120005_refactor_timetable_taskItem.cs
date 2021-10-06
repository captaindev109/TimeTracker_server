using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations
{
    public partial class refactor_timetable_taskItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "TaskItemAcls");

            migrationBuilder.DropColumn(
                name: "timeTableId",
                table: "TaskItemAcls");

            migrationBuilder.AddColumn<long>(
                name: "companyId",
                table: "TimeTables",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "TimeTables",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "taskItemId",
                table: "TimeTables",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "userId",
                table: "TimeTables",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "companyId",
                table: "TimeTables");

            migrationBuilder.DropColumn(
                name: "status",
                table: "TimeTables");

            migrationBuilder.DropColumn(
                name: "taskItemId",
                table: "TimeTables");

            migrationBuilder.DropColumn(
                name: "userId",
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
    }
}
