using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeOrganizer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExchangeAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeAmount",
                table: "Transaction",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExchangeAmount",
                table: "Transaction");
        }
    }
}
