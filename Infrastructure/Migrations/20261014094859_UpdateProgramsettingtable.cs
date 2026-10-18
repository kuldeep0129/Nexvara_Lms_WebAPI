using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProgramsettingtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgramSettings_ProgramTypes_ProgramTypeId",
                table: "ProgramSettings");

            migrationBuilder.RenameColumn(
                name: "ProgramTypeId",
                table: "ProgramSettings",
                newName: "ProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_ProgramSettings_ProgramTypeId",
                table: "ProgramSettings",
                newName: "IX_ProgramSettings_ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramSettings_Programs_ProgramId",
                table: "ProgramSettings",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgramSettings_Programs_ProgramId",
                table: "ProgramSettings");

            migrationBuilder.RenameColumn(
                name: "ProgramId",
                table: "ProgramSettings",
                newName: "ProgramTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProgramSettings_ProgramId",
                table: "ProgramSettings",
                newName: "IX_ProgramSettings_ProgramTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramSettings_ProgramTypes_ProgramTypeId",
                table: "ProgramSettings",
                column: "ProgramTypeId",
                principalTable: "ProgramTypes",
                principalColumn: "Id");
        }
    }
}
