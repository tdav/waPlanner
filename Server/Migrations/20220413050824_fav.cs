using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class fav : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_favorites_tb_users_staff_id",
                table: "tb_favorites");

            migrationBuilder.AddForeignKey(
                name: "fk_tb_favorites_tb_staffs_staff_id",
                table: "tb_favorites",
                column: "staff_id",
                principalTable: "tb_staffs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_favorites_tb_staffs_staff_id",
                table: "tb_favorites");

            migrationBuilder.AddForeignKey(
                name: "fk_tb_favorites_tb_users_staff_id",
                table: "tb_favorites",
                column: "staff_id",
                principalTable: "tb_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
