using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IcedTea.Api.Data.Migrations.ErrorLog
{
    public partial class InitialErrorLogDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "icedtea");

            migrationBuilder.CreateTable(
                name: "log",
                schema: "icedtea",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    exception = table.Column<string>(type: "text", nullable: true),
                    level = table.Column<string>(type: "text", nullable: false),
                    log_event = table.Column<string>(type: "jsonb", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    message_template = table.Column<string>(type: "text", nullable: false),
                    properties = table.Column<string>(type: "jsonb", nullable: true),
                    time_stamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_log", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "log",
                schema: "icedtea");
        }
    }
}
