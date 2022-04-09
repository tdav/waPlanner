using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class parent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sp_categories_sp_global_categories_global_category_id",
                table: "sp_categories");

            migrationBuilder.DropTable(
                name: "sp_global_categories");

            migrationBuilder.RenameColumn(
                name: "global_category_id",
                table: "sp_categories",
                newName: "parent_id");

            migrationBuilder.RenameIndex(
                name: "ix_sp_categories_global_category_id",
                table: "sp_categories",
                newName: "ix_sp_categories_parent_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sp_categories_sp_categories_parent_id",
                table: "sp_categories",
                column: "parent_id",
                principalTable: "sp_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sp_categories_sp_categories_parent_id",
                table: "sp_categories");

            migrationBuilder.RenameColumn(
                name: "parent_id",
                table: "sp_categories",
                newName: "global_category_id");

            migrationBuilder.RenameIndex(
                name: "ix_sp_categories_parent_id",
                table: "sp_categories",
                newName: "ix_sp_categories_global_category_id");

            migrationBuilder.CreateTable(
                name: "sp_global_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    name_lt = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    name_ru = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    name_uz = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    update_user = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sp_global_categories", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "fk_sp_categories_sp_global_categories_global_category_id",
                table: "sp_categories",
                column: "global_category_id",
                principalTable: "sp_global_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
