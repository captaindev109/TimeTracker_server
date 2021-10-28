using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations
{
    public partial class update_tag_field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "Tags");

            migrationBuilder.AddColumn<long>(
                name: "companyId",
                table: "Tags",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "companyId",
                table: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "Tags",
                type: "text",
                nullable: true);
        }
    }
}
