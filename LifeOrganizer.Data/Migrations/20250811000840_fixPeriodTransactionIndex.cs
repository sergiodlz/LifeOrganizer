using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeOrganizer.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixPeriodTransactionIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BudgetPeriodTransaction_BudgetPeriodId_TransactionId",
                table: "BudgetPeriodTransaction");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPeriodTransaction_BudgetPeriodId_TransactionId_IsDele~",
                table: "BudgetPeriodTransaction",
                columns: new[] { "BudgetPeriodId", "TransactionId", "IsDeleted" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BudgetPeriodTransaction_BudgetPeriodId_TransactionId_IsDele~",
                table: "BudgetPeriodTransaction");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPeriodTransaction_BudgetPeriodId_TransactionId",
                table: "BudgetPeriodTransaction",
                columns: new[] { "BudgetPeriodId", "TransactionId" },
                unique: true);
        }
    }
}
