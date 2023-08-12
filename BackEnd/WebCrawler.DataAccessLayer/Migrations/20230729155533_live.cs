using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrawler.DataAccessLayer.Migrations
{
    public partial class live : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExecutionId",
                table: "Nodes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionId",
                table: "Nodes");
        }
    }
}
