using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class re : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_schedulers_tb_users_doctor_id",
                table: "tb_schedulers");

            migrationBuilder.DropForeignKey(
                name: "fk_tb_users_sp_categories_category_id",
                table: "tb_users");

            migrationBuilder.DropForeignKey(
                name: "fk_tb_users_sp_organizations_organization_id",
                table: "tb_users");

            migrationBuilder.DropForeignKey(
                name: "fk_tb_users_sp_user_types_user_type_id",
                table: "tb_users");

            migrationBuilder.DropIndex(
                name: "ix_tb_users_category_id",
                table: "tb_users");

            migrationBuilder.DropIndex(
                name: "ix_tb_users_organization_id",
                table: "tb_users");

            migrationBuilder.DropIndex(
                name: "ix_tb_users_password",
                table: "tb_users");

            migrationBuilder.DropIndex(
                name: "ix_tb_users_user_type_id",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "ad_info",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "availability",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "experience",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "online",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "organization_id",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "password",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "photo",
                table: "tb_users");

            migrationBuilder.DropColumn(
                name: "user_type_id",
                table: "tb_users");

            migrationBuilder.RenameColumn(
                name: "doctor_id",
                table: "tb_schedulers",
                newName: "staff_id");

            migrationBuilder.RenameIndex(
                name: "ix_tb_schedulers_doctor_id",
                table: "tb_schedulers",
                newName: "ix_tb_schedulers_staff_id");

            migrationBuilder.CreateTable(
                name: "tb_staffs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    surname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    patronymic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone_num = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    password = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    birth_day = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    user_type_id = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: true),
                    organization_id = table.Column<int>(type: "integer", nullable: true),
                    telegram_id = table.Column<long>(type: "bigint", nullable: true),
                    online = table.Column<bool>(type: "boolean", nullable: true),
                    experience = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    availability = table.Column<int[]>(type: "integer[]", nullable: true),
                    photo = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ad_info = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_user = table.Column<int>(type: "integer", nullable: true),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_staffs", x => x.id);
                    table.ForeignKey(
                        name: "fk_tb_staffs_sp_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "sp_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tb_staffs_sp_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "sp_organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tb_staffs_category_id",
                table: "tb_staffs",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_staffs_organization_id",
                table: "tb_staffs",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_staffs_password",
                table: "tb_staffs",
                column: "password");

            migrationBuilder.CreateIndex(
                name: "ix_tb_staffs_phone_num",
                table: "tb_staffs",
                column: "phone_num");

            migrationBuilder.AddForeignKey(
                name: "fk_tb_schedulers_tb_staffs_staff_id",
                table: "tb_schedulers",
                column: "staff_id",
                principalTable: "tb_staffs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_schedulers_tb_staffs_staff_id",
                table: "tb_schedulers");

            migrationBuilder.DropTable(
                name: "tb_staffs");

            migrationBuilder.RenameColumn(
                name: "staff_id",
                table: "tb_schedulers",
                newName: "doctor_id");

            migrationBuilder.RenameIndex(
                name: "ix_tb_schedulers_staff_id",
                table: "tb_schedulers",
                newName: "ix_tb_schedulers_doctor_id");

            migrationBuilder.AddColumn<string>(
                name: "ad_info",
                table: "tb_users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "availability",
                table: "tb_users",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "tb_users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "experience",
                table: "tb_users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "online",
                table: "tb_users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "organization_id",
                table: "tb_users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "tb_users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "photo",
                table: "tb_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "user_type_id",
                table: "tb_users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_tb_users_category_id",
                table: "tb_users",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_users_organization_id",
                table: "tb_users",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_users_password",
                table: "tb_users",
                column: "password");

            migrationBuilder.CreateIndex(
                name: "ix_tb_users_user_type_id",
                table: "tb_users",
                column: "user_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tb_schedulers_tb_users_doctor_id",
                table: "tb_schedulers",
                column: "doctor_id",
                principalTable: "tb_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tb_users_sp_categories_category_id",
                table: "tb_users",
                column: "category_id",
                principalTable: "sp_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tb_users_sp_organizations_organization_id",
                table: "tb_users",
                column: "organization_id",
                principalTable: "sp_organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tb_users_sp_user_types_user_type_id",
                table: "tb_users",
                column: "user_type_id",
                principalTable: "sp_user_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
