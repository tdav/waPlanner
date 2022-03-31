﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class tbscheduler_category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "tb_schedulers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_tb_schedulers_category_id",
                table: "tb_schedulers",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tb_schedulers_sp_categories_category_id",
                table: "tb_schedulers",
                column: "category_id",
                principalTable: "sp_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_schedulers_sp_categories_category_id",
                table: "tb_schedulers");

            migrationBuilder.DropIndex(
                name: "ix_tb_schedulers_category_id",
                table: "tb_schedulers");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "tb_schedulers");
        }
    }
}
