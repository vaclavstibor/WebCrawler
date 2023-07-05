using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrawler.DataAccessLayer.Migrations
{
    public partial class Crawler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StartingNodes_WebsiteRecordId",
                table: "StartingNodes");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "Nodes");

            migrationBuilder.RenameColumn(
                name: "StartingNodeId",
                table: "StartingNodes",
                newName: "NodeId");

            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "Nodes",
                newName: "CrawlTime");

            migrationBuilder.AlterColumn<int>(
                name: "ExecutionStatus",
                table: "Records",
                type: "int",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RegExpMatch",
                table: "Nodes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Nodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StartingNodes_NodeId",
                table: "StartingNodes",
                column: "NodeId");

            migrationBuilder.CreateIndex(
                name: "IX_StartingNodes_WebsiteRecordId",
                table: "StartingNodes",
                column: "WebsiteRecordId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StartingNodes_Nodes_NodeId",
                table: "StartingNodes",
                column: "NodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StartingNodes_Nodes_NodeId",
                table: "StartingNodes");

            migrationBuilder.DropIndex(
                name: "IX_StartingNodes_NodeId",
                table: "StartingNodes");

            migrationBuilder.DropIndex(
                name: "IX_StartingNodes_WebsiteRecordId",
                table: "StartingNodes");

            migrationBuilder.DropColumn(
                name: "RegExpMatch",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Nodes");

            migrationBuilder.RenameColumn(
                name: "NodeId",
                table: "StartingNodes",
                newName: "StartingNodeId");

            migrationBuilder.RenameColumn(
                name: "CrawlTime",
                table: "Nodes",
                newName: "DateTime");

            migrationBuilder.AlterColumn<bool>(
                name: "ExecutionStatus",
                table: "Records",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Group",
                table: "Nodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StartingNodes_WebsiteRecordId",
                table: "StartingNodes",
                column: "WebsiteRecordId");
        }
    }
}
