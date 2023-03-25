using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookGenerator.Persistence.Migrations
{
    public partial class ProgressNavigationPropery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_BookProgresses_Books_BookId",
                table: "BookProgresses",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookProgresses_Books_BookId",
                table: "BookProgresses");
        }
    }
}
