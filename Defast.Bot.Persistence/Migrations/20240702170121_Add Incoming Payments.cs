using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Defast.Bot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIncomingPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncomingPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocType = table.Column<string>(type: "text", nullable: true),
                    DocDate = table.Column<string>(type: "text", nullable: true),
                    DocNum = table.Column<int>(type: "integer", nullable: true),
                    CashAccount = table.Column<string>(type: "text", nullable: true),
                    DocCurrency = table.Column<string>(type: "text", nullable: false),
                    CashSum = table.Column<decimal>(type: "numeric", nullable: false),
                    CashSumFC = table.Column<decimal>(type: "numeric", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    CardCode = table.Column<string>(type: "text", nullable: false),
                    CardName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingPayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncomingPaymentAccounts",
                columns: table => new
                {
                    IncomingPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountCode = table.Column<string>(type: "text", nullable: false),
                    SumPaid = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingPaymentAccounts", x => x.IncomingPaymentId);
                    table.ForeignKey(
                        name: "FK_IncomingPaymentAccounts_IncomingPayments_IncomingPaymentId",
                        column: x => x.IncomingPaymentId,
                        principalTable: "IncomingPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentInvoice",
                columns: table => new
                {
                    IncomingPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LineNum = table.Column<int>(type: "integer", nullable: false),
                    DocEntry = table.Column<int>(type: "integer", nullable: false),
                    DocNum = table.Column<int>(type: "integer", nullable: false),
                    SumApplied = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentInvoice", x => x.IncomingPaymentId);
                    table.ForeignKey(
                        name: "FK_PaymentInvoice_IncomingPayments_IncomingPaymentId",
                        column: x => x.IncomingPaymentId,
                        principalTable: "IncomingPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingPaymentAccounts");

            migrationBuilder.DropTable(
                name: "PaymentInvoice");

            migrationBuilder.DropTable(
                name: "IncomingPayments");
        }
    }
}
