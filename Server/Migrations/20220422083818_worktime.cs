using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class worktime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "dinner_time_start",
                table: "sp_organizations",
                newName: "work_start");

            migrationBuilder.RenameColumn(
                name: "dinner_time_end",
                table: "sp_organizations",
                newName: "work_end");

            migrationBuilder.AddColumn<DateTime>(
                name: "break_time_end",
                table: "sp_organizations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "break_time_start",
                table: "sp_organizations",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "break_time_end",
                table: "sp_organizations");

            migrationBuilder.DropColumn(
                name: "break_time_start",
                table: "sp_organizations");

            migrationBuilder.RenameColumn(
                name: "work_start",
                table: "sp_organizations",
                newName: "dinner_time_start");

            migrationBuilder.RenameColumn(
                name: "work_end",
                table: "sp_organizations",
                newName: "dinner_time_end");
        }
    }
}
