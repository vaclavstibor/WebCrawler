using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrawler.DataAccessLayer.Migrations
{
    public partial class CrawlerUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebsiteRecordId",
                table: "Nodes");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExecutionStart",
                table: "Records",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionStart",
                table: "Records");

            migrationBuilder.AddColumn<int>(
                name: "WebsiteRecordId",
                table: "Nodes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
