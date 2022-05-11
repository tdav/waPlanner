using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sp_setups_tb_users_user_id",
                table: "sp_setups");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "sp_setups",
                newName: "staff_id");

            migrationBuilder.RenameIndex(
                name: "ix_sp_setups_user_id",
                table: "sp_setups",
                newName: "ix_sp_setups_staff_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sp_setups_tb_staffs_staff_id",
                table: "sp_setups",
                column: "staff_id",
                principalTable: "tb_staffs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sp_setups_tb_staffs_staff_id",
                table: "sp_setups");

            migrationBuilder.RenameColumn(
                name: "staff_id",
                table: "sp_setups",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_sp_setups_staff_id",
                table: "sp_setups",
                newName: "ix_sp_setups_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sp_setups_tb_users_user_id",
                table: "sp_setups",
                column: "user_id",
                principalTable: "tb_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
