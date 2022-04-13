﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using waPlanner.Database;

#nullable disable

namespace waPlanner.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20220413071044_del")]
    partial class del
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("waPlanner.Database.Models.spCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date");

                    b.Property<int>("CreateUser")
                        .HasColumnType("integer")
                        .HasColumnName("create_user");

                    b.Property<string>("NameLt")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name_lt");

                    b.Property<string>("NameRu")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name_ru");

                    b.Property<string>("NameUz")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name_uz");

                    b.Property<int?>("OrganizationId")
                        .HasColumnType("integer")
                        .HasColumnName("organization_id");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date");

                    b.Property<int?>("UpdateUser")
                        .HasColumnType("integer")
                        .HasColumnName("update_user");

                    b.HasKey("Id")
                        .HasName("pk_sp_categories");

                    b.HasIndex("NameLt")
                        .IsUnique()
                        .HasDatabaseName("ix_sp_categories_name_lt");

                    b.HasIndex("NameRu")
                        .IsUnique()
                        .HasDatabaseName("ix_sp_categories_name_ru");

                    b.HasIndex("NameUz")
                        .IsUnique()
                        .HasDatabaseName("ix_sp_categories_name_uz");

                    b.HasIndex("OrganizationId")
                        .HasDatabaseName("ix_sp_categories_organization_id");

                    b.ToTable("sp_categories", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.spOrganization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("address");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint")
                        .HasColumnName("chat_id");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date");

                    b.Property<int>("CreateUser")
                        .HasColumnType("integer")
                        .HasColumnName("create_user");

                    b.Property<float>("Latitude")
                        .HasColumnType("real")
                        .HasColumnName("latitude");

                    b.Property<float>("Longitude")
                        .HasColumnType("real")
                        .HasColumnName("longitude");

                    b.Property<string>("Name")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("name");

                    b.Property<int?>("SpecializationId")
                        .HasColumnType("integer")
                        .HasColumnName("specialization_id");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date");

                    b.Property<int?>("UpdateUser")
                        .HasColumnType("integer")
                        .HasColumnName("update_user");

                    b.HasKey("Id")
                        .HasName("pk_sp_organizations");

                    b.HasIndex("SpecializationId")
                        .HasDatabaseName("ix_sp_organizations_specialization_id");

                    b.ToTable("sp_organizations", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.spRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date");

                    b.Property<int>("CreateUser")
                        .HasColumnType("integer")
                        .HasColumnName("create_user");

                    b.Property<string>("Name")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("name");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date");

                    b.Property<int?>("UpdateUser")
                        .HasColumnType("integer")
                        .HasColumnName("update_user");

                    b.HasKey("Id")
                        .HasName("pk_sp_roles");

                    b.ToTable("sp_roles", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.spSpecialization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date");

                    b.Property<int>("CreateUser")
                        .HasColumnType("integer")
                        .HasColumnName("create_user");

                    b.Property<string>("NameLt")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name_lt");

                    b.Property<string>("NameRu")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name_ru");

                    b.Property<string>("NameUz")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name_uz");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date");

                    b.Property<int?>("UpdateUser")
                        .HasColumnType("integer")
                        .HasColumnName("update_user");

                    b.HasKey("Id")
                        .HasName("pk_sp_specializations");

                    b.HasIndex("NameLt")
                        .IsUnique()
                        .HasDatabaseName("ix_sp_specializations_name_lt");

                    b.HasIndex("NameRu")
                        .IsUnique()
                        .HasDatabaseName("ix_sp_specializations_name_ru");

                    b.HasIndex("NameUz")
                        .IsUnique()
                        .HasDatabaseName("ix_sp_specializations_name_uz");

                    b.ToTable("sp_specializations", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbFavorites", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date");

                    b.Property<int>("CreateUser")
                        .HasColumnType("integer")
                        .HasColumnName("create_user");

                    b.Property<int>("OrganizationId")
                        .HasColumnType("integer")
                        .HasColumnName("organization_id");

                    b.Property<int>("StaffId")
                        .HasColumnType("integer")
                        .HasColumnName("staff_id");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_id");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date");

                    b.Property<int?>("UpdateUser")
                        .HasColumnType("integer")
                        .HasColumnName("update_user");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_tb_favorites");

                    b.HasIndex("OrganizationId")
                        .HasDatabaseName("ix_tb_favorites_organization_id");

                    b.HasIndex("StaffId")
                        .HasDatabaseName("ix_tb_favorites_staff_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_tb_favorites_user_id");

                    b.ToTable("tb_favorites", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbScheduler", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AdInfo")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("ad_info");

                    b.Property<DateTime>("AppointmentDateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("appointment_date_time");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer")
                        .HasColumnName("category_id");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date");

                    b.Property<int>("CreateUser")
                        .HasColumnType("integer")
                        .HasColumnName("create_user");

                    b.Property<int>("OrganizationId")
                        .HasColumnType("integer")
                        .HasColumnName("organization_id");

                    b.Property<int>("StaffId")
                        .HasColumnType("integer")
                        .HasColumnName("staff_id");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date");

                    b.Property<int?>("UpdateUser")
                        .HasColumnType("integer")
                        .HasColumnName("update_user");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_tb_schedulers");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("ix_tb_schedulers_category_id");

                    b.HasIndex("OrganizationId")
                        .HasDatabaseName("ix_tb_schedulers_organization_id");

                    b.HasIndex("StaffId")
                        .HasDatabaseName("ix_tb_schedulers_staff_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_tb_schedulers_user_id");

                    b.ToTable("tb_schedulers", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbStaff", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AdInfo")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("ad_info");

                    b.Property<int[]>("Availability")
                        .HasColumnType("integer[]")
                        .HasColumnName("availability");

                    b.Property<DateTime?>("BirthDay")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("birth_day");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("integer")
                        .HasColumnName("category_id");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date");

                    b.Property<int>("CreateUser")
                        .HasColumnType("integer")
                        .HasColumnName("create_user");

                    b.Property<DateTime?>("Experience")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("experience");

                    b.Property<string>("Gender")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("gender");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("name");

                    b.Property<bool?>("Online")
                        .HasColumnType("boolean")
                        .HasColumnName("online");

                    b.Property<int?>("OrganizationId")
                        .HasColumnType("integer")
                        .HasColumnName("organization_id");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("password");

                    b.Property<string>("Patronymic")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("patronymic");

                    b.Property<string>("PhoneNum")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("phone_num");

                    b.Property<string>("PhotoUrl")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("photo_url");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("surname");

                    b.Property<long?>("TelegramId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_id");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date");

                    b.Property<int?>("UpdateUser")
                        .HasColumnType("integer")
                        .HasColumnName("update_user");

                    b.HasKey("Id")
                        .HasName("pk_tb_staffs");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("ix_tb_staffs_category_id");

                    b.HasIndex("OrganizationId")
                        .HasDatabaseName("ix_tb_staffs_organization_id");

                    b.HasIndex("Password")
                        .HasDatabaseName("ix_tb_staffs_password");

                    b.HasIndex("PhoneNum")
                        .IsUnique()
                        .HasDatabaseName("ix_tb_staffs_phone_num");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_tb_staffs_role_id");

                    b.ToTable("tb_staffs", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("BirthDay")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("birth_day");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date");

                    b.Property<int>("CreateUser")
                        .HasColumnType("integer")
                        .HasColumnName("create_user");

                    b.Property<string>("Gender")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("gender");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("name");

                    b.Property<string>("Patronymic")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("patronymic");

                    b.Property<string>("PhoneNum")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("phone_num");

                    b.Property<string>("PhotoUrl")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("photo_url");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("surname");

                    b.Property<long?>("TelegramId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_id");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date");

                    b.Property<int?>("UpdateUser")
                        .HasColumnType("integer")
                        .HasColumnName("update_user");

                    b.HasKey("Id")
                        .HasName("pk_tb_users");

                    b.HasIndex("PhoneNum")
                        .HasDatabaseName("ix_tb_users_phone_num");

                    b.ToTable("tb_users", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.spCategory", b =>
                {
                    b.HasOne("waPlanner.Database.Models.spOrganization", "Organization")
                        .WithMany("Categories")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_sp_categories_sp_organizations_organization_id");

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("waPlanner.Database.Models.spOrganization", b =>
                {
                    b.HasOne("waPlanner.Database.Models.spSpecialization", "Specialization")
                        .WithMany("Organizations")
                        .HasForeignKey("SpecializationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_sp_organizations_sp_specializations_specialization_id");

                    b.Navigation("Specialization");
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbFavorites", b =>
                {
                    b.HasOne("waPlanner.Database.Models.spOrganization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_favorites_sp_organizations_organization_id");

                    b.HasOne("waPlanner.Database.Models.tbStaff", "Staff")
                        .WithMany()
                        .HasForeignKey("StaffId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_favorites_tb_staffs_staff_id");

                    b.HasOne("waPlanner.Database.Models.tbUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_favorites_tb_users_user_id");

                    b.Navigation("Organization");

                    b.Navigation("Staff");

                    b.Navigation("User");
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbScheduler", b =>
                {
                    b.HasOne("waPlanner.Database.Models.spCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_schedulers_sp_categories_category_id");

                    b.HasOne("waPlanner.Database.Models.spOrganization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_schedulers_sp_organizations_organization_id");

                    b.HasOne("waPlanner.Database.Models.tbStaff", "Staff")
                        .WithMany()
                        .HasForeignKey("StaffId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_schedulers_tb_staffs_staff_id");

                    b.HasOne("waPlanner.Database.Models.tbUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_schedulers_tb_users_user_id");

                    b.Navigation("Category");

                    b.Navigation("Organization");

                    b.Navigation("Staff");

                    b.Navigation("User");
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbStaff", b =>
                {
                    b.HasOne("waPlanner.Database.Models.spCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_tb_staffs_sp_categories_category_id");

                    b.HasOne("waPlanner.Database.Models.spOrganization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_tb_staffs_sp_organizations_organization_id");

                    b.HasOne("waPlanner.Database.Models.spRole", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_staffs_sp_roles_role_id");

                    b.Navigation("Category");

                    b.Navigation("Organization");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("waPlanner.Database.Models.spOrganization", b =>
                {
                    b.Navigation("Categories");
                });

            modelBuilder.Entity("waPlanner.Database.Models.spSpecialization", b =>
                {
                    b.Navigation("Organizations");
                });
#pragma warning restore 612, 618
        }
    }
}
