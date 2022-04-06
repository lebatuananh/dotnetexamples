using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GithubTrending.Api.Data.Migrations.GithubTrending
{
    public partial class InitialGithubTrending : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "github_repositories");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "github_trending",
                schema: "github_repositories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    owner = table.Column<string>(type: "text", nullable: true),
                    repository = table.Column<string>(type: "text", nullable: true),
                    language = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    star_count = table.Column<string>(type: "text", nullable: true),
                    fork_count = table.Column<string>(type: "text", nullable: true),
                    today_star_count = table.Column<string>(type: "text", nullable: true),
                    owners_twitter_account = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    status_github_trending = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_updated_by = table.Column<string>(type: "text", nullable: true),
                    last_updated_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_github_trending", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "github_trending",
                schema: "github_repositories");
        }
    }
}
