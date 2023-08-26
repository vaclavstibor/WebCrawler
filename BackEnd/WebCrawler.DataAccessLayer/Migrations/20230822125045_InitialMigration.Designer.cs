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
    [Migration("20230822125045_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("NodeNode", b =>
                {
                    b.Property<int>("ChildrenId")
                        .HasColumnType("int");

                    b.Property<int>("ParentsId")
                        .HasColumnType("int");

                    b.HasKey("ChildrenId", "ParentsId");

                    b.HasIndex("ParentsId");

                    b.ToTable("NodeNode");
                });

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.Execution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("ExecutionStatus")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfSites")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("WebsiteRecordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WebsiteRecordId");

                    b.ToTable("Executions");
                });

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.Node", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<TimeSpan>("CrawlTime")
                        .HasColumnType("time");

                    b.Property<string>("Domain")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ExecutionId")
                        .HasColumnType("int");

                    b.Property<bool?>("RegExpMatch")
                        .HasColumnType("bit");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WebsiteRecordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Nodes");
                });

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.StartingNode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("NodeId")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfSites")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NodeId");

                    b.ToTable("StartingNodes");
                });

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

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

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

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

            modelBuilder.Entity("NodeNode", b =>
                {
                    b.HasOne("WebCrawler.DataAccessLayer.Models.Node", null)
                        .WithMany()
                        .HasForeignKey("ChildrenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebCrawler.DataAccessLayer.Models.Node", null)
                        .WithMany()
                        .HasForeignKey("ParentsId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.Execution", b =>
                {
                    b.HasOne("WebCrawler.DataAccessLayer.Models.WebsiteRecord", "WebsiteRecord")
                        .WithMany()
                        .HasForeignKey("WebsiteRecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WebsiteRecord");
                });

            modelBuilder.Entity("WebCrawler.DataAccessLayer.Models.StartingNode", b =>
                {
                    b.HasOne("WebCrawler.DataAccessLayer.Models.Node", "Node")
                        .WithMany()
                        .HasForeignKey("NodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Node");
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