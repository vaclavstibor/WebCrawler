﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebCrawler.DataAccessLayer.Context;

#nullable disable

namespace WebCrawler.DataAccessLayer.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220607165224_nullable")]
    partial class nullable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WebsiteRecordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WebsiteRecordId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.WebsiteRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int?>("Days")
                        .HasColumnType("int");

                    b.Property<bool?>("ExecutionStatus")
                        .HasColumnType("bit");

                    b.Property<int?>("Hours")
                        .HasColumnType("int");

                    b.Property<string>("Label")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastExecution")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Minutes")
                        .HasColumnType("int");

                    b.Property<string>("RegExp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("URL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.Tag", b =>
                {
                    b.HasOne("WebCrawler.DataAccessLayer.Models.WebsiteRecord", "WebsiteRecord")
                        .WithMany("Tags")
                        .HasForeignKey("WebsiteRecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WebsiteRecord");
                });

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.WebsiteRecord", b =>
                {
                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
