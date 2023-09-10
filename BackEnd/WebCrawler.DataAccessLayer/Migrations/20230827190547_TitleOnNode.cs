using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrawler.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class TitleOnNode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Nodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Nodes");
        }
    }
}
