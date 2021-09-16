﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TimeTracker_server.Data;

namespace TimeTracker_server.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20210916042746_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("TimeTracker_server.Models.Company", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("create_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<string>("status")
                        .HasColumnType("text");

                    b.Property<DateTime>("update_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("TimeTracker_server.Models.Project", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("create_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("createdBy")
                        .HasColumnType("bigint");

                    b.Property<string>("description")
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<DateTime>("planEnd")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("planStart")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("publicStatus")
                        .HasColumnType("text");

                    b.Property<string>("status")
                        .HasColumnType("text");

                    b.Property<DateTime>("update_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("updatedBy")
                        .HasColumnType("bigint");

                    b.HasKey("id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("TimeTracker_server.Models.ResourceType", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("company")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("create_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("createdBy")
                        .HasColumnType("bigint");

                    b.Property<string>("description")
                        .HasColumnType("text");

                    b.Property<float>("hourlyRate")
                        .HasColumnType("real");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<string>("status")
                        .HasColumnType("text");

                    b.Property<List<string>>("tags")
                        .HasColumnType("text[]");

                    b.Property<DateTime>("update_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("updatedBy")
                        .HasColumnType("bigint");

                    b.HasKey("id");

                    b.ToTable("ResourceTypes");
                });

            modelBuilder.Entity("TimeTracker_server.Models.TaskItem", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("company")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("create_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("createdBy")
                        .HasColumnType("bigint");

                    b.Property<string>("description")
                        .HasColumnType("text");

                    b.Property<long>("duration")
                        .HasColumnType("bigint");

                    b.Property<long>("end")
                        .HasColumnType("bigint");

                    b.Property<long>("project")
                        .HasColumnType("bigint");

                    b.Property<long>("start")
                        .HasColumnType("bigint");

                    b.Property<string>("status")
                        .HasColumnType("text");

                    b.Property<List<string>>("tags")
                        .HasColumnType("text[]");

                    b.Property<string>("title")
                        .HasColumnType("text");

                    b.Property<DateTime>("update_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("updatedBy")
                        .HasColumnType("bigint");

                    b.HasKey("id");

                    b.ToTable("TaskItems");
                });

            modelBuilder.Entity("TimeTracker_server.Models.TaskType", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("create_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("createdBy")
                        .HasColumnType("bigint");

                    b.Property<string>("description")
                        .HasColumnType("text");

                    b.Property<string>("status")
                        .HasColumnType("text");

                    b.Property<List<string>>("tags")
                        .HasColumnType("text[]");

                    b.Property<string>("title")
                        .HasColumnType("text");

                    b.Property<DateTime>("update_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("updatedBy")
                        .HasColumnType("bigint");

                    b.HasKey("id");

                    b.ToTable("TaskTypes");
                });

            modelBuilder.Entity("TimeTracker_server.Models.Team", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("create_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("createdBy")
                        .HasColumnType("bigint");

                    b.Property<string>("description")
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<string>("status")
                        .HasColumnType("text");

                    b.Property<DateTime>("update_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("updatedBy")
                        .HasColumnType("bigint");

                    b.HasKey("id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("TimeTracker_server.Models.TimeTable", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("description")
                        .HasColumnType("text");

                    b.Property<long>("duration")
                        .HasColumnType("bigint");

                    b.Property<long>("end")
                        .HasColumnType("bigint");

                    b.Property<long>("start")
                        .HasColumnType("bigint");

                    b.Property<long>("taskItem")
                        .HasColumnType("bigint");

                    b.HasKey("id");

                    b.ToTable("TimeTables");
                });

            modelBuilder.Entity("TimeTracker_server.Models.User", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("avatar")
                        .HasColumnType("text");

                    b.Property<DateTime>("create_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("email")
                        .HasColumnType("text");

                    b.Property<string>("firstName")
                        .HasColumnType("text");

                    b.Property<bool>("isVerified")
                        .HasColumnType("boolean");

                    b.Property<string>("lastName")
                        .HasColumnType("text");

                    b.Property<string>("password")
                        .HasColumnType("text");

                    b.Property<string>("role")
                        .HasColumnType("text");

                    b.Property<string>("status")
                        .HasColumnType("text");

                    b.Property<DateTime>("update_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TimeTracker_server.Models.UserAcl", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("create_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("objectId")
                        .HasColumnType("bigint");

                    b.Property<string>("objectType")
                        .HasColumnType("text");

                    b.Property<string>("role")
                        .HasColumnType("text");

                    b.Property<long>("sourceId")
                        .HasColumnType("bigint");

                    b.Property<string>("sourceType")
                        .HasColumnType("text");

                    b.Property<DateTime>("update_timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("id");

                    b.ToTable("UserAcls");
                });
#pragma warning restore 612, 618
        }
    }
}
