using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IcedTea.Api.Data.Migrations
{
    public partial class InitialIcedTeaDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "icedtea");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "cash_funds",
                schema: "icedtea",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_updated_by = table.Column<string>(type: "text", nullable: true),
                    last_updated_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cash_funds", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "wallet",
                schema: "icedtea",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    sub_total_amount = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wallet", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cash_fund_transactions",
                schema: "icedtea",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false),
                    payment_gateway = table.Column<int>(type: "integer", nullable: false),
                    completed_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    cash_fund_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_updated_by = table.Column<string>(type: "text", nullable: true),
                    last_updated_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cash_fund_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_cash_fund_transactions_cash_funds_cash_fund_id",
                        column: x => x.cash_fund_id,
                        principalSchema: "icedtea",
                        principalTable: "cash_funds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customer",
                schema: "icedtea",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    user_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    device_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_updated_by = table.Column<string>(type: "text", nullable: true),
                    last_updated_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer", x => x.id);
                    table.ForeignKey(
                        name: "fk_customer_wallets_wallet_id",
                        column: x => x.wallet_id,
                        principalSchema: "icedtea",
                        principalTable: "wallet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                schema: "icedtea",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: false),
                    bank_account = table.Column<string>(type: "text", nullable: false),
                    completed_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    response = table.Column<string>(type: "text", nullable: false),
                    payment_gateway = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cash_fund_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_updated_by = table.Column<string>(type: "text", nullable: true),
                    last_updated_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_transactions_cash_funds_cash_fund_id",
                        column: x => x.cash_fund_id,
                        principalSchema: "icedtea",
                        principalTable: "cash_funds",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_transactions_customers_customer_id",
                        column: x => x.customer_id,
                        principalSchema: "icedtea",
                        principalTable: "customer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cash_fund_transactions_cash_fund_id",
                schema: "icedtea",
                table: "cash_fund_transactions",
                column: "cash_fund_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_wallet_id",
                schema: "icedtea",
                table: "customer",
                column: "wallet_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_cash_fund_id",
                schema: "icedtea",
                table: "transactions",
                column: "cash_fund_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_customer_id",
                schema: "icedtea",
                table: "transactions",
                column: "customer_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cash_fund_transactions",
                schema: "icedtea");

            migrationBuilder.DropTable(
                name: "transactions",
                schema: "icedtea");

            migrationBuilder.DropTable(
                name: "cash_funds",
                schema: "icedtea");

            migrationBuilder.DropTable(
                name: "customer",
                schema: "icedtea");

            migrationBuilder.DropTable(
                name: "wallet",
                schema: "icedtea");
        }
    }
}
