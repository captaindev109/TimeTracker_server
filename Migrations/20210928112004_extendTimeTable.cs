using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations
{
    public partial class extendTimeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "duration",
                table: "TimeTables",
                newName: "pauseStart");

            migrationBuilder.AddColumn<long>(
                name: "pauseDuration",
                table: "TimeTables",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pauseDuration",
                table: "TimeTables");

            migrationBuilder.RenameColumn(
                name: "pauseStart",
                table: "TimeTables",
                newName: "duration");
        }
    }
}
