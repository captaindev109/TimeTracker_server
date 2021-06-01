using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations.TaskItem
{
    public partial class UpdateField_TaskItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "position",
                table: "TaskItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tag",
                table: "TaskItems",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "position",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "tag",
                table: "TaskItems");
        }
    }
}
