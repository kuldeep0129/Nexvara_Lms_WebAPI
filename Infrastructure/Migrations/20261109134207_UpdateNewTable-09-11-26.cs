using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNewTable091126 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BatchClassSessions_CourseModules_CourseModuleId",
                table: "BatchClassSessions");

            migrationBuilder.AddForeignKey(
                name: "FK_BatchClassSessions_CourseSyllabus_CourseModuleId",
                table: "BatchClassSessions",
                column: "CourseModuleId",
                principalTable: "CourseSyllabus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BatchClassSessions_CourseSyllabus_CourseModuleId",
                table: "BatchClassSessions");

            migrationBuilder.AddForeignKey(
                name: "FK_BatchClassSessions_CourseModules_CourseModuleId",
                table: "BatchClassSessions",
                column: "CourseModuleId",
                principalTable: "CourseModules",
                principalColumn: "Id");
        }
    }
}
