using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations.ResourceType
{
    public partial class UpdateFeild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tag",
                table: "ResourceTypes",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tag",
                table: "ResourceTypes");
        }
    }
}
