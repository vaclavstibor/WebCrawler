using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrawler.DataAccessLayer.Migrations
{
    public partial class StartingNodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Nodes_StartingNodeId",
                table: "Records");

            migrationBuilder.DropIndex(
                name: "IX_Records_StartingNodeId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "StartingNodeId",
                table: "Records");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "Nodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "Nodes");

            migrationBuilder.AddColumn<int>(
                name: "StartingNodeId",
                table: "Records",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Records_StartingNodeId",
                table: "Records",
                column: "StartingNodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Nodes_StartingNodeId",
                table: "Records",
                column: "StartingNodeId",
                principalTable: "Nodes",
                principalColumn: "Id");
        }
    }
}
