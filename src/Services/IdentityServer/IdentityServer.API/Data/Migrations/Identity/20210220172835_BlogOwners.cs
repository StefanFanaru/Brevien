using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.API.Data.Migrations.Identity
{
    public partial class BlogOwners : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BlogId",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BlogId",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Blogs",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Blogs", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "BlogOwners",
                schema: "identity",
                columns: table => new
                {
                    BlogId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogOwners", x => new {x.UserId, x.BlogId});
                    table.ForeignKey(
                        name: "FK_BlogOwners_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalSchema: "identity",
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogOwners_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogOwners_BlogId",
                schema: "identity",
                table: "BlogOwners",
                column: "BlogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogOwners",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Blogs",
                schema: "identity");

            migrationBuilder.AddColumn<string>(
                name: "BlogId",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BlogId",
                schema: "identity",
                table: "AspNetUsers",
                column: "BlogId",
                unique: true,
                filter: "[BlogId] IS NOT NULL");
        }
    }
}