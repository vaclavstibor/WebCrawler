using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrawler.DataAccessLayer.Migrations
{
    public partial class MultipleParents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Nodes_ParentId",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_ParentId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Nodes");

            migrationBuilder.CreateTable(
                name: "NodeNode",
                columns: table => new
                {
                    ChildrenId = table.Column<int>(type: "int", nullable: false),
                    ParentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeNode", x => new { x.ChildrenId, x.ParentsId });
                    table.ForeignKey(
                        name: "FK_NodeNode_Nodes_ChildrenId",
                        column: x => x.ChildrenId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NodeNode_Nodes_ParentsId",
                        column: x => x.ParentsId,
                        principalTable: "Nodes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NodeNode_ParentsId",
                table: "NodeNode",
                column: "ParentsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodeNode");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Nodes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ParentId",
                table: "Nodes",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Nodes_ParentId",
                table: "Nodes",
                column: "ParentId",
                principalTable: "Nodes",
                principalColumn: "Id");
        }
    }
}
