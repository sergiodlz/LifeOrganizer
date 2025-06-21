using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeOrganizer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSubcategoryUniqueIndexWithIsDeletedFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subcategory_UserId_Name_CategoryId",
                table: "Subcategory");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategory_UserId_Name_CategoryId",
                table: "Subcategory",
                columns: new[] { "UserId", "Name", "CategoryId" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subcategory_UserId_Name_CategoryId",
                table: "Subcategory");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategory_UserId_Name_CategoryId",
                table: "Subcategory",
                columns: new[] { "UserId", "Name", "CategoryId" },
                unique: true);
        }
    }
}
