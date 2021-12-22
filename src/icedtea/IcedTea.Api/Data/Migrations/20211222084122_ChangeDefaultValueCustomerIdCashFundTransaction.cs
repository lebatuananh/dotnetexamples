using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IcedTea.Api.Data.Migrations
{
    public partial class ChangeDefaultValueCustomerIdCashFundTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                schema: "icedtea",
                table: "cash_fund_transactions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                schema: "icedtea",
                table: "cash_fund_transactions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
