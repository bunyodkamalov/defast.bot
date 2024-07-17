using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Defast.Bot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddcashFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "U_cashFlow",
                table: "IncomingPayments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "U_cashFlow",
                table: "IncomingPayments");
        }
    }
}
