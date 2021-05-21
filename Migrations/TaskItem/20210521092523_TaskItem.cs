using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TimeTracker_server.Migrations.TaskItem
{
    public partial class TaskItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskItems",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    project = table.Column<long>(type: "bigint", nullable: false),
                    company = table.Column<long>(type: "bigint", nullable: false),
                    createdBy = table.Column<long>(type: "bigint", nullable: false),
                    updatedBy = table.Column<long>(type: "bigint", nullable: false),
                    duration = table.Column<long>(type: "bigint", nullable: false),
                    start = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    create_timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    update_timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItems", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskItems");
        }
    }
}
