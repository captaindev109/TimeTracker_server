using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations.UserAcl
{
    public partial class fieldUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userId",
                table: "UserAcls",
                newName: "sourceId");

            migrationBuilder.AddColumn<string>(
                name: "sourceType",
                table: "UserAcls",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sourceType",
                table: "UserAcls");

            migrationBuilder.RenameColumn(
                name: "sourceId",
                table: "UserAcls",
                newName: "userId");
        }
    }
}
