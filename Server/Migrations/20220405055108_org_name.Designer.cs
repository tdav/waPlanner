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
    [Migration("20220405055108_org_name")]
    partial class org_name
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

                    b.Property<int?>("GlobalCategoryId")
                        .HasColumnType("integer")
                        .HasColumnName("global_category_id");

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

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date");

                    b.Property<int?>("UpdateUser")
                        .HasColumnType("integer")
                        .HasColumnName("update_user");

                    b.HasKey("Id")
                        .HasName("pk_sp_categories");

                    b.HasIndex("GlobalCategoryId")
                        .HasDatabaseName("ix_sp_categories_global_category_id");

                    b.ToTable("sp_categories", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.spGlobalCategory", b =>
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

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date");

                    b.Property<int?>("UpdateUser")
                        .HasColumnType("integer")
                        .HasColumnName("update_user");

                    b.HasKey("Id")
                        .HasName("pk_sp_global_categories");

                    b.ToTable("sp_global_categories", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.spOrganizationType", b =>
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
                        .HasName("pk_sp_organization_types");

                    b.ToTable("sp_organization_types", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.spUserType", b =>
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
                        .HasName("pk_sp_user_types");

                    b.ToTable("sp_user_types", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbOrganization", b =>
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

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<int>("TypeId")
                        .HasColumnType("integer")
                        .HasColumnName("type_id");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date");

                    b.Property<int?>("UpdateUser")
                        .HasColumnType("integer")
                        .HasColumnName("update_user");

                    b.HasKey("Id")
                        .HasName("pk_tb_organizations");

                    b.HasIndex("TypeId")
                        .HasDatabaseName("ix_tb_organizations_type_id");

                    b.ToTable("tb_organizations", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbScheduler", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

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

                    b.Property<int>("DoctorId")
                        .HasColumnType("integer")
                        .HasColumnName("doctor_id");

                    b.Property<string>("DpInfo")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("dp_info");

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

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_tb_schedulers");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("ix_tb_schedulers_category_id");

                    b.HasIndex("DoctorId")
                        .HasDatabaseName("ix_tb_schedulers_doctor_id");

                    b.HasIndex("OrganizationId")
                        .HasDatabaseName("ix_tb_schedulers_organization_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_tb_schedulers_user_id");

                    b.ToTable("tb_schedulers", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

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
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("patronymic");

                    b.Property<string>("PhoneNum")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("phone_num");

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

                    b.Property<int>("UserTypeId")
                        .HasColumnType("integer")
                        .HasColumnName("user_type_id");

                    b.HasKey("Id")
                        .HasName("pk_tb_users");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("ix_tb_users_category_id");

                    b.HasIndex("OrganizationId")
                        .HasDatabaseName("ix_tb_users_organization_id");

                    b.HasIndex("Password")
                        .HasDatabaseName("ix_tb_users_password");

                    b.HasIndex("PhoneNum")
                        .HasDatabaseName("ix_tb_users_phone_num");

                    b.HasIndex("UserTypeId")
                        .HasDatabaseName("ix_tb_users_user_type_id");

                    b.ToTable("tb_users", (string)null);
                });

            modelBuilder.Entity("waPlanner.Database.Models.spCategory", b =>
                {
                    b.HasOne("waPlanner.Database.Models.spGlobalCategory", "GlobalCategory")
                        .WithMany("Categories")
                        .HasForeignKey("GlobalCategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_sp_categories_sp_global_categories_global_category_id");

                    b.Navigation("GlobalCategory");
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbOrganization", b =>
                {
                    b.HasOne("waPlanner.Database.Models.spOrganizationType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_organizations_sp_organization_types_type_id");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbScheduler", b =>
                {
                    b.HasOne("waPlanner.Database.Models.spCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_schedulers_sp_categories_category_id");

                    b.HasOne("waPlanner.Database.Models.tbUser", "Doctor")
                        .WithMany()
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_schedulers_tb_users_doctor_id");

                    b.HasOne("waPlanner.Database.Models.tbOrganization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_tb_schedulers_tb_organizations_organization_id");

                    b.HasOne("waPlanner.Database.Models.tbUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_schedulers_tb_users_user_id");

                    b.Navigation("Category");

                    b.Navigation("Doctor");

                    b.Navigation("Organization");

                    b.Navigation("User");
                });

            modelBuilder.Entity("waPlanner.Database.Models.tbUser", b =>
                {
                    b.HasOne("waPlanner.Database.Models.spCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_tb_users_sp_categories_category_id");

                    b.HasOne("waPlanner.Database.Models.tbOrganization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_tb_users_tb_organizations_organization_id");

                    b.HasOne("waPlanner.Database.Models.spUserType", "UserType")
                        .WithMany()
                        .HasForeignKey("UserTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tb_users_sp_user_types_user_type_id");

                    b.Navigation("Category");

                    b.Navigation("Organization");

                    b.Navigation("UserType");
                });

            modelBuilder.Entity("waPlanner.Database.Models.spGlobalCategory", b =>
                {
                    b.Navigation("Categories");
                });
#pragma warning restore 612, 618
        }
    }
}
