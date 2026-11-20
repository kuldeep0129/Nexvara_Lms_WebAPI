using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCourseVideo10112026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseVideos_CourseModules_ModuleId",
                table: "CourseVideos");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseVideos_CourseSyllabus_ModuleId",
                table: "CourseVideos",
                column: "ModuleId",
                principalTable: "CourseSyllabus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseVideos_CourseSyllabus_ModuleId",
                table: "CourseVideos");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseVideos_CourseModules_ModuleId",
                table: "CourseVideos",
                column: "ModuleId",
                principalTable: "CourseModules",
                principalColumn: "Id");
        }
    }
}
