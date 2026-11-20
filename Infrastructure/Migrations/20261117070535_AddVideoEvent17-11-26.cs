using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoEvent171126 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastWatchedAt",
                table: "VideoProgresses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "LastEventId",
                table: "VideoProgresses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "VideoProgresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSessionId",
                table: "VideoProgresses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VideoProgressEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    VideoId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionSeconds = table.Column<int>(type: "int", nullable: true),
                    WatchedSecondsDelta = table.Column<int>(type: "int", nullable: true),
                    NetworkStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StopReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoProgressEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoProgressEvents_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VideoProgressEvents_CourseVideos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "CourseVideos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VideoWatchSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    VideoId = table.Column<int>(type: "int", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastHeartbeatAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPositionSeconds = table.Column<int>(type: "int", nullable: true),
                    IsPlaying = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StopReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoWatchSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoWatchSessions_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VideoWatchSessions_CourseVideos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "CourseVideos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoProgressEvents_StudentId",
                table: "VideoProgressEvents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoProgressEvents_VideoId",
                table: "VideoProgressEvents",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoWatchSessions_StudentId",
                table: "VideoWatchSessions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoWatchSessions_VideoId",
                table: "VideoWatchSessions",
                column: "VideoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoProgressEvents");

            migrationBuilder.DropTable(
                name: "VideoWatchSessions");

            migrationBuilder.DropColumn(
                name: "LastEventId",
                table: "VideoProgresses");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "VideoProgresses");

            migrationBuilder.DropColumn(
                name: "LastSessionId",
                table: "VideoProgresses");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastWatchedAt",
                table: "VideoProgresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
