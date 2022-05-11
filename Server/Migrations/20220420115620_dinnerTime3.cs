using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class dinnerTime3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "dinner_time_end",
                table: "sp_organizations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "dinner_time_start",
                table: "sp_organizations",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dinner_time_end",
                table: "sp_organizations");

            migrationBuilder.DropColumn(
                name: "dinner_time_start",
                table: "sp_organizations");
        }
    }
}
