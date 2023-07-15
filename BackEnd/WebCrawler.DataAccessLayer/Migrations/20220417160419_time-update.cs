using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrawler.DataAccessLayer.Migrations
{
    public partial class timeupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Periodicity",
                table: "Records");

            migrationBuilder.AddColumn<int>(
                name: "Days",
                table: "Records",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Hours",
                table: "Records",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Minutes",
                table: "Records",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Days",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "Minutes",
                table: "Records");

            migrationBuilder.AddColumn<DateTime>(
                name: "Periodicity",
                table: "Records",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
