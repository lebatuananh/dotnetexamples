using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IcedTea.Api.Data.Migrations
{
    public partial class AddInfomationCustomerCashFundTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "customer_id",
                schema: "icedtea",
                table: "cash_fund_transactions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<string>(
                name: "customer_name",
                schema: "icedtea",
                table: "cash_fund_transactions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customer_id",
                schema: "icedtea",
                table: "cash_fund_transactions");

            migrationBuilder.DropColumn(
                name: "customer_name",
                schema: "icedtea",
                table: "cash_fund_transactions");
        }
    }
}
