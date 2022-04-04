using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class new1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "symptoms",
                table: "tb_schedulers",
                newName: "dp_info");

            migrationBuilder.AddColumn<int>(
                name: "organization_id",
                table: "tb_schedulers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_tb_schedulers_organization_id",
                table: "tb_schedulers",
                column: "organization_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tb_schedulers_tb_organizations_organization_id",
                table: "tb_schedulers",
                column: "organization_id",
                principalTable: "tb_organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_schedulers_tb_organizations_organization_id",
                table: "tb_schedulers");

            migrationBuilder.DropIndex(
                name: "ix_tb_schedulers_organization_id",
                table: "tb_schedulers");

            migrationBuilder.DropColumn(
                name: "organization_id",
                table: "tb_schedulers");

            migrationBuilder.RenameColumn(
                name: "dp_info",
                table: "tb_schedulers",
                newName: "symptoms");
        }
    }
}
