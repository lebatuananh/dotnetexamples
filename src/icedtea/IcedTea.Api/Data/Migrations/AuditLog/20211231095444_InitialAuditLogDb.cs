using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IcedTea.Api.Data.Migrations.AuditLog
{
    public partial class InitialAuditLogDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auditlog");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "audit_log",
                schema: "auditlog",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    action = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data = table.Column<string>(type: "text", nullable: true),
                    @event = table.Column<string>(name: "event", type: "text", nullable: true),
                    source = table.Column<string>(type: "text", nullable: true),
                    subject_additional_data = table.Column<string>(type: "text", nullable: true),
                    subject_identifier = table.Column<string>(type: "text", nullable: true),
                    subject_name = table.Column<string>(type: "text", nullable: true),
                    subject_type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table => { table.PrimaryKey("pk_audit_log", x => x.id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_log",
                schema: "auditlog");
        }
    }
}