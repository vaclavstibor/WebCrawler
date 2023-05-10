using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrawler.DataAccessLayer.Migrations
{
    public partial class recordidinnodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WebsiteRecordId",
                table: "Nodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StartingNodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartingNodeId = table.Column<int>(type: "int", nullable: false),
                    WebsiteRecordId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StartingNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StartingNodes_Records_WebsiteRecordId",
                        column: x => x.WebsiteRecordId,
                        principalTable: "Records",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StartingNodes_WebsiteRecordId",
                table: "StartingNodes",
                column: "WebsiteRecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StartingNodes");

            migrationBuilder.DropColumn(
                name: "WebsiteRecordId",
                table: "Nodes");
        }
    }
}
