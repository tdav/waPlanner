using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class org_type_in_users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "organization_id",
                table: "tb_users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_tb_users_organization_id",
                table: "tb_users",
                column: "organization_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tb_users_tb_organizations_organization_id",
                table: "tb_users",
                column: "organization_id",
                principalTable: "tb_organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_users_tb_organizations_organization_id",
                table: "tb_users");

            migrationBuilder.DropIndex(
                name: "ix_tb_users_organization_id",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "organization_id",
                table: "tb_users");
        }
    }
}
