using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class categories_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_sp_categories_name_lt",
                table: "sp_categories",
                column: "name_lt",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sp_categories_name_ru",
                table: "sp_categories",
                column: "name_ru",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sp_categories_name_uz",
                table: "sp_categories",
                column: "name_uz",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sp_categories_name_lt",
                table: "sp_categories");

            migrationBuilder.DropIndex(
                name: "ix_sp_categories_name_ru",
                table: "sp_categories");

            migrationBuilder.DropIndex(
                name: "ix_sp_categories_name_uz",
                table: "sp_categories");
        }
    }
}
