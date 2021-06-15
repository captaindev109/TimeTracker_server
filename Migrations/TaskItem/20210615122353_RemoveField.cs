using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations.TaskItem
{
    public partial class RemoveField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "position",
                table: "TaskItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "position",
                table: "TaskItems",
                type: "text",
                nullable: true);
        }
    }
}
