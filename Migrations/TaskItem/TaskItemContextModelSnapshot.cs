﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TaskItemApi.Models;

namespace TimeTracker_server.Migrations.TaskItem
{
    [DbContext(typeof(TaskItemContext))]
    partial class TaskItemContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("TaskItemApi.Models.TaskItem", b =>
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
#pragma warning restore 612, 618
        }
    }
}
