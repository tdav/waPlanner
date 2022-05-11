using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class seria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "passport_seria",
                table: "tb_users",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pinfl",
                table: "tb_users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "passport_seria",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "pinfl",
                table: "tb_users");
        }
    }
}
