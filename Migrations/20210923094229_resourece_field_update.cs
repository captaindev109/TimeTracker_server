using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations
{
    public partial class resourece_field_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "company",
                table: "ResourceTypes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "company",
                table: "ResourceTypes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
