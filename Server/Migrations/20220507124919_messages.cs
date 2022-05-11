using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class messages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "message_lt",
                table: "sp_organizations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "message_ru",
                table: "sp_organizations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "message_uz",
                table: "sp_organizations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "message_lt",
                table: "sp_organizations");

            migrationBuilder.DropColumn(
                name: "message_ru",
                table: "sp_organizations");

            migrationBuilder.DropColumn(
                name: "message_uz",
                table: "sp_organizations");
        }
    }
}
