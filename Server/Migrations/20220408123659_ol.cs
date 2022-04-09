using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class ol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_schedulers_tb_organizations_organization_id",
                table: "tb_schedulers");

            migrationBuilder.DropForeignKey(
                name: "fk_tb_users_tb_organizations_organization_id",
                table: "tb_users");

            migrationBuilder.DropTable(
                name: "tb_organizations");

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
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: false),
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
                        name: "fk_sp_organizations_sp_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "sp_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_sp_organizations_sp_organization_types_type_id",
                        column: x => x.type_id,
                        principalTable: "sp_organization_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sp_organizations_category_id",
                table: "sp_organizations",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_sp_organizations_type_id",
                table: "sp_organizations",
                column: "type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tb_schedulers_sp_organizations_organization_id",
                table: "tb_schedulers",
                column: "organization_id",
                principalTable: "sp_organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tb_users_sp_organizations_organization_id",
                table: "tb_users",
                column: "organization_id",
                principalTable: "sp_organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_schedulers_sp_organizations_organization_id",
                table: "tb_schedulers");

            migrationBuilder.DropForeignKey(
                name: "fk_tb_users_sp_organizations_organization_id",
                table: "tb_users");

            migrationBuilder.DropTable(
                name: "sp_organizations");

            migrationBuilder.CreateTable(
                name: "tb_organizations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    create_user = table.Column<int>(type: "integer", nullable: false),
                    latitude = table.Column<float>(type: "real", nullable: false),
                    longitude = table.Column<float>(type: "real", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    update_user = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_organizations", x => x.id);
                    table.ForeignKey(
                        name: "fk_tb_organizations_sp_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "sp_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tb_organizations_sp_organization_types_type_id",
                        column: x => x.type_id,
                        principalTable: "sp_organization_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tb_organizations_category_id",
                table: "tb_organizations",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_organizations_type_id",
                table: "tb_organizations",
                column: "type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tb_schedulers_tb_organizations_organization_id",
                table: "tb_schedulers",
                column: "organization_id",
                principalTable: "tb_organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tb_users_tb_organizations_organization_id",
                table: "tb_users",
                column: "organization_id",
                principalTable: "tb_organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
