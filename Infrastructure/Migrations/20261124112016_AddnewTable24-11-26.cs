using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddnewTable241126 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseVideoId",
                table: "ClassAttendances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BatchSessionAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchClassSessionId = table.Column<int>(type: "int", nullable: true),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarkedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MarkedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchSessionAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchSessionAttendances_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchSessionAttendances_BatchClassSessions_BatchClassSessionId",
                        column: x => x.BatchClassSessionId,
                        principalTable: "BatchClassSessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassAttendances_CourseVideoId",
                table: "ClassAttendances",
                column: "CourseVideoId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchSessionAttendances_BatchClassSessionId",
                table: "BatchSessionAttendances",
                column: "BatchClassSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchSessionAttendances_StudentId",
                table: "BatchSessionAttendances",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassAttendances_CourseVideos_CourseVideoId",
                table: "ClassAttendances",
                column: "CourseVideoId",
                principalTable: "CourseVideos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassAttendances_CourseVideos_CourseVideoId",
                table: "ClassAttendances");

            migrationBuilder.DropTable(
                name: "BatchSessionAttendances");

            migrationBuilder.DropIndex(
                name: "IX_ClassAttendances_CourseVideoId",
                table: "ClassAttendances");

            migrationBuilder.DropColumn(
                name: "CourseVideoId",
                table: "ClassAttendances");
        }
    }
}
