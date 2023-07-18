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
    [Migration("20230705154608_Crawler")]
    partial class Crawler
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.Node", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CrawlTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Domain")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("NodeId")
                        .HasColumnType("int");

                    b.Property<bool?>("RegExpMatch")
                        .HasColumnType("bit");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WebsiteRecordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NodeId");

                    b.ToTable("Nodes");
                });

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.StartingNode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("NodeId")
                        .HasColumnType("int");

                    b.Property<int>("WebsiteRecordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NodeId");

                    b.HasIndex("WebsiteRecordId")
                        .IsUnique();

                    b.ToTable("StartingNodes");
                });

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

                    b.Property<int?>("ExecutionStatus")
                        .HasColumnType("int");

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

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.Node", b =>
                {
                    b.HasOne("WebCrawler.DataAccessLayer.Models.Node", null)
                        .WithMany("Children")
                        .HasForeignKey("NodeId");
                });

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.StartingNode", b =>
                {
                    b.HasOne("WebCrawler.DataAccessLayer.Models.Node", "Node")
                        .WithMany()
                        .HasForeignKey("NodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebCrawler.DataAccessLayer.Models.WebsiteRecord", "WebsiteRecord")
                        .WithOne("StartingNode")
                        .HasForeignKey("WebCrawler.DataAccessLayer.Models.StartingNode", "WebsiteRecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Node");

                    b.Navigation("WebsiteRecord");
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

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.Node", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.WebsiteRecord", b =>
                {
                    b.Navigation("StartingNode")
                        .IsRequired();

                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}