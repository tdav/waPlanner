using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class org_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "tb_organizations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                table: "tb_organizations");
        }
    }
}
