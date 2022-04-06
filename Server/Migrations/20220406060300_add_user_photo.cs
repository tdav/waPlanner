using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class add_user_photo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "dp_info",
                table: "tb_schedulers",
                newName: "ad_info");

            migrationBuilder.AddColumn<string>(
                name: "ad_info",
                table: "tb_users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "photo",
                table: "tb_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ad_info",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "photo",
                table: "tb_users");

            migrationBuilder.RenameColumn(
                name: "ad_info",
                table: "tb_schedulers",
                newName: "dp_info");
        }
    }
}
