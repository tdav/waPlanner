﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace waPlanner.Migrations
{
    public partial class del : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sp_user_types");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sp_user_types",
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
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    update_user = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sp_user_types", x => x.id);
                });
        }
    }
}
