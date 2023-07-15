using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrawler.DataAccessLayer.Migrations
{
    public partial class nodetimechangedtotimespan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "CrawlTime",
                table: "Nodes",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CrawlTime",
                table: "Nodes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");
        }
    }
}
