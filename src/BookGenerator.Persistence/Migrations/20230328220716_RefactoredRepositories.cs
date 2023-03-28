using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookGenerator.Persistence.Migrations
{
    public partial class RefactoredRepositories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookProgresses_Books_BookId",
                table: "BookProgresses");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "BookProgresses",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookProgresses_Books_Id",
                table: "BookProgresses",
                column: "Id",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookProgresses_Books_Id",
                table: "BookProgresses");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BookProgresses",
                newName: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookProgresses_Books_BookId",
                table: "BookProgresses",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
