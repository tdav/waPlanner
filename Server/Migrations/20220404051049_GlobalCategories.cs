using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class GlobalCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tip",
                table: "sp_categories",
                newName: "type");

            migrationBuilder.AddColumn<int>(
                name: "global_category_id",
                table: "sp_categories",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sp_global_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    update_user = table.Column<int>(type: "integer", nullable: true),
                    name_uz = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    name_lt = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    name_ru = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sp_global_categories", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sp_categories_global_category_id",
                table: "sp_categories",
                column: "global_category_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sp_categories_sp_global_categories_global_category_id",
                table: "sp_categories",
                column: "global_category_id",
                principalTable: "sp_global_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sp_categories_sp_global_categories_global_category_id",
                table: "sp_categories");

            migrationBuilder.DropTable(
                name: "sp_global_categories");

            migrationBuilder.DropIndex(
                name: "ix_sp_categories_global_category_id",
                table: "sp_categories");

            migrationBuilder.DropColumn(
                name: "global_category_id",
                table: "sp_categories");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "sp_categories",
                newName: "tip");
        }
    }
}
