using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "sp_specializations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name_uz = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    name_lt = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    name_ru = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    update_user = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sp_specializations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    surname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    patronymic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    birth_day = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    phone_num = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    telegram_id = table.Column<long>(type: "bigint", nullable: true),
                    photo_url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    pinfl = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    passport_seria = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_user = table.Column<int>(type: "integer", nullable: true),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sp_organizations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    address = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    info = table.Column<string>(type: "text", nullable: true),
                    latitude = table.Column<float>(type: "real", nullable: false),
                    longitude = table.Column<float>(type: "real", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    break_time_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    break_time_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    work_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    work_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    message_uz = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    message_ru = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    message_lt = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    specialization_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_user = table.Column<int>(type: "integer", nullable: true),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sp_organizations", x => x.id);
                    table.ForeignKey(
                        name: "fk_sp_organizations_sp_specializations_specialization_id",
                        column: x => x.specialization_id,
                        principalTable: "sp_specializations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sp_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name_uz = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    name_lt = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    name_ru = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    organization_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    update_user = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sp_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_sp_categories_sp_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "sp_organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: true),
                    organization_id = table.Column<int>(type: "integer", nullable: true),
                    telegram_id = table.Column<long>(type: "bigint", nullable: true),
                    online = table.Column<bool>(type: "boolean", nullable: true),
                    experience = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    availability = table.Column<int[]>(type: "integer[]", nullable: true),
                    photo_url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ad_info = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
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
                    table.ForeignKey(
                        name: "fk_tb_staffs_sp_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "sp_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sp_setups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    staff_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    text = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_user = table.Column<int>(type: "integer", nullable: true),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sp_setups", x => x.id);
                    table.ForeignKey(
                        name: "fk_sp_setups_tb_staffs_staff_id",
                        column: x => x.staff_id,
                        principalTable: "tb_staffs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_sp_setups_tb_users_user_id",
                        column: x => x.user_id,
                        principalTable: "tb_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_analize_results",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    url = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    ad_info = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    staff_id = table.Column<int>(type: "integer", nullable: true),
                    organization_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_user = table.Column<int>(type: "integer", nullable: true),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_analize_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_tb_analize_results_sp_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "sp_organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tb_analize_results_tb_staffs_staff_id",
                        column: x => x.staff_id,
                        principalTable: "tb_staffs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tb_analize_results_tb_users_user_id",
                        column: x => x.user_id,
                        principalTable: "tb_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_favorites",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    telegram_id = table.Column<long>(type: "bigint", nullable: false),
                    staff_id = table.Column<int>(type: "integer", nullable: true),
                    organization_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_user = table.Column<int>(type: "integer", nullable: true),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_favorites", x => x.id);
                    table.ForeignKey(
                        name: "fk_tb_favorites_sp_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "sp_organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tb_favorites_tb_staffs_staff_id",
                        column: x => x.staff_id,
                        principalTable: "tb_staffs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tb_favorites_tb_users_user_id",
                        column: x => x.user_id,
                        principalTable: "tb_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_schedulers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    staff_id = table.Column<int>(type: "integer", nullable: false),
                    appointment_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ad_info = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_user = table.Column<int>(type: "integer", nullable: true),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_schedulers", x => x.id);
                    table.ForeignKey(
                        name: "fk_tb_schedulers_sp_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "sp_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tb_schedulers_sp_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "sp_organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tb_schedulers_tb_staffs_staff_id",
                        column: x => x.staff_id,
                        principalTable: "tb_staffs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tb_schedulers_tb_users_user_id",
                        column: x => x.user_id,
                        principalTable: "tb_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sp_categories_organization_id",
                table: "sp_categories",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_sp_organizations_specialization_id",
                table: "sp_organizations",
                column: "specialization_id");

            migrationBuilder.CreateIndex(
                name: "ix_sp_setups_staff_id",
                table: "sp_setups",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "ix_sp_setups_user_id",
                table: "sp_setups",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_sp_specializations_name_lt",
                table: "sp_specializations",
                column: "name_lt",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sp_specializations_name_ru",
                table: "sp_specializations",
                column: "name_ru",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sp_specializations_name_uz",
                table: "sp_specializations",
                column: "name_uz",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tb_analize_results_organization_id",
                table: "tb_analize_results",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_analize_results_staff_id",
                table: "tb_analize_results",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_analize_results_user_id",
                table: "tb_analize_results",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_favorites_organization_id",
                table: "tb_favorites",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_favorites_staff_id",
                table: "tb_favorites",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_favorites_user_id",
                table: "tb_favorites",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_schedulers_category_id",
                table: "tb_schedulers",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_schedulers_organization_id",
                table: "tb_schedulers",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_schedulers_staff_id",
                table: "tb_schedulers",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_schedulers_user_id",
                table: "tb_schedulers",
                column: "user_id");

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

            migrationBuilder.CreateIndex(
                name: "ix_tb_staffs_role_id",
                table: "tb_staffs",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_users_phone_num",
                table: "tb_users",
                column: "phone_num");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sp_setups");

            migrationBuilder.DropTable(
                name: "tb_analize_results");

            migrationBuilder.DropTable(
                name: "tb_favorites");

            migrationBuilder.DropTable(
                name: "tb_schedulers");

            migrationBuilder.DropTable(
                name: "tb_staffs");

            migrationBuilder.DropTable(
                name: "tb_users");

            migrationBuilder.DropTable(
                name: "sp_categories");

            migrationBuilder.DropTable(
                name: "sp_roles");

            migrationBuilder.DropTable(
                name: "sp_organizations");

            migrationBuilder.DropTable(
                name: "sp_specializations");
        }
    }
}
