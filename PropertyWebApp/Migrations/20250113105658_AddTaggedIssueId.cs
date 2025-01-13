using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTaggedIssueId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaggedIssues",
                table: "TaggedIssues");

            migrationBuilder.AddColumn<int>(
                name: "TaggedIssueId",
                table: "TaggedIssues",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaggedIssues",
                table: "TaggedIssues",
                column: "TaggedIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_TaggedIssues_IssueId",
                table: "TaggedIssues",
                column: "IssueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaggedIssues",
                table: "TaggedIssues");

            migrationBuilder.DropIndex(
                name: "IX_TaggedIssues_IssueId",
                table: "TaggedIssues");

            migrationBuilder.DropColumn(
                name: "TaggedIssueId",
                table: "TaggedIssues");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaggedIssues",
                table: "TaggedIssues",
                column: "IssueId");
        }
    }
}
