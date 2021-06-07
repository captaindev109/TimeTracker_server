﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ResourceTypeApi.Models;

namespace TimeTracker_server.Migrations.ResourceType
{
    [DbContext(typeof(ResourceTypeContext))]
    [Migration("20210607064626_UpdateField")]
    partial class UpdateField
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("ResourceTypeApi.Models.ResourceType", b =>
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
#pragma warning restore 612, 618
        }
    }
}
