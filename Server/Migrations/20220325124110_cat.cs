using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class cat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "tb_users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_tb_users_category_id",
                table: "tb_users",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tb_users_sp_categories_category_id",
                table: "tb_users",
                column: "category_id",
                principalTable: "sp_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_users_sp_categories_category_id",
                table: "tb_users");

            migrationBuilder.DropIndex(
                name: "ix_tb_users_category_id",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "tb_users");
        }
    }
}
