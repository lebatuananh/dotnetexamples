﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Shared.Logging.LogError;

#nullable disable

namespace GithubTrending.Api.Data.Migrations.ErrorLog
{
    [DbContext(typeof(LogDbContext))]
    partial class LogDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Shared.Logging.LogError.Log", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Exception")
                        .HasColumnType("text")
                        .HasColumnName("exception");

                    b.Property<string>("Level")
                        .HasColumnType("text")
                        .HasColumnName("level");

                    b.Property<string>("LogEvent")
                        .HasColumnType("jsonb")
                        .HasColumnName("log_event");

                    b.Property<string>("Message")
                        .HasColumnType("text")
                        .HasColumnName("message");

                    b.Property<string>("MessageTemplate")
                        .HasColumnType("text")
                        .HasColumnName("message_template");

                    b.Property<string>("Properties")
                        .HasColumnType("jsonb")
                        .HasColumnName("properties");

                    b.Property<DateTimeOffset>("TimeStamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("time_stamp");

                    b.HasKey("Id")
                        .HasName("pk_log");

                    b.ToTable("log", "error_log");
                });
#pragma warning restore 612, 618
        }
    }
}
