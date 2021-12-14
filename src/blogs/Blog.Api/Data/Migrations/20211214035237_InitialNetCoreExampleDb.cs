using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Api.Data.Migrations
{
    public partial class InitialNetCoreExampleDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "blog");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "blog",
                schema: "blog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    poster = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_updated_by = table.Column<string>(type: "text", nullable: true),
                    last_updated_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_blog", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tag",
                schema: "blog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_updated_by = table.Column<string>(type: "text", nullable: true),
                    last_updated_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "blog_tag",
                schema: "blog",
                columns: table => new
                {
                    tag_id = table.Column<Guid>(type: "uuid", nullable: false),
                    blog_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_blog_tag", x => new { x.blog_id, x.tag_id });
                    table.ForeignKey(
                        name: "fk_blog_tag_blog_blog_id",
                        column: x => x.blog_id,
                        principalSchema: "blog",
                        principalTable: "blog",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_blog_tag_tags_tag_id",
                        column: x => x.tag_id,
                        principalSchema: "blog",
                        principalTable: "tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_blog_tag_tag_id",
                schema: "blog",
                table: "blog_tag",
                column: "tag_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blog_tag",
                schema: "blog");

            migrationBuilder.DropTable(
                name: "blog",
                schema: "blog");

            migrationBuilder.DropTable(
                name: "tag",
                schema: "blog");
        }
    }
}
