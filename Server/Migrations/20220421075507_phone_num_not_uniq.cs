using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class phone_num_not_uniq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tb_staffs_phone_num",
                table: "tb_staffs");

            migrationBuilder.CreateIndex(
                name: "ix_tb_staffs_phone_num",
                table: "tb_staffs",
                column: "phone_num");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tb_staffs_phone_num",
                table: "tb_staffs");

            migrationBuilder.CreateIndex(
                name: "ix_tb_staffs_phone_num",
                table: "tb_staffs",
                column: "phone_num",
                unique: true);
        }
    }
}
