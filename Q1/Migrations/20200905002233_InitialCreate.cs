using Microsoft.EntityFrameworkCore.Migrations;

namespace Q1.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DirectoryItems",
                columns: table => new
                {
                    DirectoryItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Notes = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    FullPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryItems", x => x.DirectoryItemId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UserDirectoryItems",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    DirectoryItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDirectoryItems", x => new { x.UserId, x.DirectoryItemId });
                    table.ForeignKey(
                        name: "FK_UserDirectoryItems_DirectoryItems_DirectoryItemId",
                        column: x => x.DirectoryItemId,
                        principalTable: "DirectoryItems",
                        principalColumn: "DirectoryItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDirectoryItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDirectoryItems_DirectoryItemId",
                table: "UserDirectoryItems",
                column: "DirectoryItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDirectoryItems");

            migrationBuilder.DropTable(
                name: "DirectoryItems");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
