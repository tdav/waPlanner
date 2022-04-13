using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class token : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tb_staffs_phone_num",
                table: "tb_staffs");

            migrationBuilder.RenameColumn(
                name: "user_type_id",
                table: "tb_staffs",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "photo",
                table: "tb_staffs",
                newName: "photo_url");

            migrationBuilder.AddColumn<string>(
                name: "photo_url",
                table: "tb_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sp_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_user = table.Column<int>(type: "integer", nullable: true),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sp_roles", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tb_staffs_phone_num",
                table: "tb_staffs",
                column: "phone_num",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tb_staffs_role_id",
                table: "tb_staffs",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tb_staffs_sp_roles_role_id",
                table: "tb_staffs",
                column: "role_id",
                principalTable: "sp_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_staffs_sp_roles_role_id",
                table: "tb_staffs");

            migrationBuilder.DropTable(
                name: "sp_roles");

            migrationBuilder.DropIndex(
                name: "ix_tb_staffs_phone_num",
                table: "tb_staffs");

            migrationBuilder.DropIndex(
                name: "ix_tb_staffs_role_id",
                table: "tb_staffs");

            migrationBuilder.DropColumn(
                name: "photo_url",
                table: "tb_users");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "tb_staffs",
                newName: "user_type_id");

            migrationBuilder.RenameColumn(
                name: "photo_url",
                table: "tb_staffs",
                newName: "photo");

            migrationBuilder.CreateIndex(
                name: "ix_tb_staffs_phone_num",
                table: "tb_staffs",
                column: "phone_num");
        }
    }
}
