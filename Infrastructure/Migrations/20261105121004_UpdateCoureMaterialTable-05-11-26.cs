using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCoureMaterialTable051126 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseMaterials_CourseModules_ModuleId",
                table: "CourseMaterials");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseMaterials_CourseSyllabus_ModuleId",
                table: "CourseMaterials",
                column: "ModuleId",
                principalTable: "CourseSyllabus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseMaterials_CourseSyllabus_ModuleId",
                table: "CourseMaterials");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseMaterials_CourseModules_ModuleId",
                table: "CourseMaterials",
                column: "ModuleId",
                principalTable: "CourseModules",
                principalColumn: "Id");
        }
    }
}
