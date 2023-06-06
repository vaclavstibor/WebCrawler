﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrawler.DataAccessLayer.Migrations
{
    public partial class changeTagsTypeInRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExecutionStatus",
                table: "Records",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastExecution",
                table: "Records",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionStatus",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "LastExecution",
                table: "Records");
        }
    }
}
