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
                name: "sp_user_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                    table.PrimaryKey("pk_sp_user_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sp_organizations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    address = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    latitude = table.Column<float>(type: "real", nullable: false),
                    longitude = table.Column<float>(type: "real", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
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
                name: "tb_users",
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
                    table.PrimaryKey("pk_tb_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_tb_users_sp_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "sp_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tb_users_sp_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "sp_organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tb_users_sp_user_types_user_type_id",
                        column: x => x.user_type_id,
                        principalTable: "sp_user_types",
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
                    doctor_id = table.Column<int>(type: "integer", nullable: false),
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
                        name: "fk_tb_schedulers_tb_users_doctor_id",
                        column: x => x.doctor_id,
                        principalTable: "tb_users",
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
                name: "ix_sp_categories_name_lt",
                table: "sp_categories",
                column: "name_lt",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sp_categories_name_ru",
                table: "sp_categories",
                column: "name_ru",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sp_categories_name_uz",
                table: "sp_categories",
                column: "name_uz",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sp_categories_organization_id",
                table: "sp_categories",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_sp_organizations_specialization_id",
                table: "sp_organizations",
                column: "specialization_id");

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
                name: "ix_tb_schedulers_category_id",
                table: "tb_schedulers",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_schedulers_doctor_id",
                table: "tb_schedulers",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_schedulers_organization_id",
                table: "tb_schedulers",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_schedulers_user_id",
                table: "tb_schedulers",
                column: "user_id");

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
                name: "ix_tb_users_phone_num",
                table: "tb_users",
                column: "phone_num");

            migrationBuilder.CreateIndex(
                name: "ix_tb_users_user_type_id",
                table: "tb_users",
                column: "user_type_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_schedulers");

            migrationBuilder.DropTable(
                name: "tb_users");

            migrationBuilder.DropTable(
                name: "sp_categories");

            migrationBuilder.DropTable(
                name: "sp_user_types");

            migrationBuilder.DropTable(
                name: "sp_organizations");

            migrationBuilder.DropTable(
                name: "sp_specializations");
        }
    }
}
