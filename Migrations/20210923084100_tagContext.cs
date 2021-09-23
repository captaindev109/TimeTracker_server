using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TimeTracker_server.Migrations
{
    public partial class tagContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tags",
                table: "TaskTypes");

            migrationBuilder.DropColumn(
                name: "tags",
                table: "ResourceTypes");

            migrationBuilder.CreateTable(
                name: "TagAcls",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tagId = table.Column<long>(type: "bigint", nullable: false),
                    objectId = table.Column<long>(type: "bigint", nullable: false),
                    objectType = table.Column<string>(type: "text", nullable: true),
                    create_timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    update_timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagAcls", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true),
                    create_timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    update_timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagAcls");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.AddColumn<string[]>(
                name: "tags",
                table: "TaskTypes",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "tags",
                table: "ResourceTypes",
                type: "text[]",
                nullable: true);
        }
    }
}
