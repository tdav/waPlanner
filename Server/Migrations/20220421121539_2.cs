using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "staff_id",
                table: "sp_setups",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "sp_setups",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_sp_setups_user_id",
                table: "sp_setups",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sp_setups_tb_users_user_id",
                table: "sp_setups",
                column: "user_id",
                principalTable: "tb_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sp_setups_tb_users_user_id",
                table: "sp_setups");

            migrationBuilder.DropIndex(
                name: "ix_sp_setups_user_id",
                table: "sp_setups");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "sp_setups");

            migrationBuilder.AlterColumn<int>(
                name: "staff_id",
                table: "sp_setups",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
