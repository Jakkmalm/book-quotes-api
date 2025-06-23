using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookQuotesApi.Migrations
{
    /// <inheritdoc />
    public partial class LinkBooksToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Books",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Books_AppUserId",
                table: "Books",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Users_AppUserId",
                table: "Books",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Users_AppUserId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_AppUserId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Books");
        }
    }
}
