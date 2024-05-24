using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bt_api.Migrations
{
    public partial class addBugReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BugReportDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugReportDbSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugReportDbSet_UserDbSet_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BugReportDbSet_UserId",
                table: "BugReportDbSet",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugReportDbSet");
        }
    }
}
