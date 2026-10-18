using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyOrgnizationTable161026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AspNetUserId",
                table: "Organizations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_AspNetUserId",
                table: "Organizations",
                column: "AspNetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_AspNetUsers_AspNetUserId",
                table: "Organizations",
                column: "AspNetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_AspNetUsers_AspNetUserId",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_AspNetUserId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "AspNetUserId",
                table: "Organizations");
        }
    }
}
