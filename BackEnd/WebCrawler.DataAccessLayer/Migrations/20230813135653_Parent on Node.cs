using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrawler.DataAccessLayer.Migrations
{
    public partial class ParentonNode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Nodes_NodeId",
                table: "Nodes");

            migrationBuilder.RenameColumn(
                name: "NodeId",
                table: "Nodes",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Nodes_NodeId",
                table: "Nodes",
                newName: "IX_Nodes_ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Nodes_ParentId",
                table: "Nodes",
                column: "ParentId",
                principalTable: "Nodes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Nodes_ParentId",
                table: "Nodes");

            migrationBuilder.DropTable(
                name: "StartingNodes");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Nodes",
                newName: "NodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Nodes_ParentId",
                table: "Nodes",
                newName: "IX_Nodes_NodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Nodes_NodeId",
                table: "Nodes",
                column: "NodeId",
                principalTable: "Nodes",
                principalColumn: "Id");
        }
    }
}
