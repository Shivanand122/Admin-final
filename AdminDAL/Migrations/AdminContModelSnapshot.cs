﻿// <auto-generated />
using System;
using AdminDAL.Entities2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AdminDAL.Migrations
{
    [DbContext(typeof(AdminCont))]
    partial class AdminContModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.HasSequence<int>("SalesOrderNumber", "SalesLT");

            modelBuilder.Entity("AdminDAL.Entities2.EntityTbl", b =>
                {
                    b.Property<string>("EntityName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EntityName");

                    b.ToTable("EntityTbl", (string)null);
                });

            modelBuilder.Entity("AdminDAL.Entities2.Feature", b =>
                {
                    b.Property<int>("FeatureId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("FeatureID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FeatureId"));

                    b.Property<string>("AdminComments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte>("ApprovalStatus")
                        .HasColumnType("tinyint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("EntityName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FeatureDataType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FeatureName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValueSql("(N'')");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FeatureId");

                    b.HasIndex(new[] { "EntityName" }, "IX_Features_EntityName");

                    b.ToTable("Features");
                });

            modelBuilder.Entity("AdminDAL.Entities2.UserAdminRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AdminUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AdminUserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserAdminRole", (string)null);
                });

            modelBuilder.Entity("AdminDAL.Entities2.Feature", b =>
                {
                    b.HasOne("AdminDAL.Entities2.EntityTbl", "EntityNameNavigation")
                        .WithMany("Features")
                        .HasForeignKey("EntityName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EntityNameNavigation");
                });

            modelBuilder.Entity("AdminDAL.Entities2.EntityTbl", b =>
                {
                    b.Navigation("Features");
                });
#pragma warning restore 612, 618
        }
    }
}
