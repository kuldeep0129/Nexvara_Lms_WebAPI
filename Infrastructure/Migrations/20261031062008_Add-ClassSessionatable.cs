using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClassSessionatable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BatchClassSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: true),
                    BatchScheduleId = table.Column<int>(type: "int", nullable: true),
                    SessionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    TeacherId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CourseModuleId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReschedule = table.Column<bool>(type: "bit", nullable: false),
                    OriginalSessionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OriginalStartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    OriginalEndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    RescheduleReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchClassSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchClassSessions_AspNetUsers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BatchClassSessions_BatchSchedules_BatchScheduleId",
                        column: x => x.BatchScheduleId,
                        principalTable: "BatchSchedules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BatchClassSessions_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BatchClassSessions_CourseModules_CourseModuleId",
                        column: x => x.CourseModuleId,
                        principalTable: "CourseModules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchClassSessions_BatchId",
                table: "BatchClassSessions",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchClassSessions_BatchScheduleId",
                table: "BatchClassSessions",
                column: "BatchScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchClassSessions_CourseModuleId",
                table: "BatchClassSessions",
                column: "CourseModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchClassSessions_TeacherId",
                table: "BatchClassSessions",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchClassSessions");
        }
    }
}
