﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations.Team
{
    public partial class fieldUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "members",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "teamLead",
                table: "Teams");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long[]>(
                name: "members",
                table: "Teams",
                type: "bigint[]",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "teamLead",
                table: "Teams",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
