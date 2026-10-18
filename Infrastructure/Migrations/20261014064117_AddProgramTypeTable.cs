using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProgramTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Programs");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Programs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DurationUnit",
                table: "Programs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Durationvalue",
                table: "Programs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Point",
                table: "Programs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgramTypeId",
                table: "Programs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WithoutFee",
                table: "Programs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProgramTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramTypeId = table.Column<int>(type: "int", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoftColorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramSettings_ProgramTypes_ProgramTypeId",
                        column: x => x.ProgramTypeId,
                        principalTable: "ProgramTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Programs_ProgramTypeId",
                table: "Programs",
                column: "ProgramTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramSettings_ProgramTypeId",
                table: "ProgramSettings",
                column: "ProgramTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_ProgramTypes_ProgramTypeId",
                table: "Programs",
                column: "ProgramTypeId",
                principalTable: "ProgramTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_ProgramTypes_ProgramTypeId",
                table: "Programs");

            migrationBuilder.DropTable(
                name: "ProgramSettings");

            migrationBuilder.DropTable(
                name: "ProgramTypes");

            migrationBuilder.DropIndex(
                name: "IX_Programs_ProgramTypeId",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "DurationUnit",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "Durationvalue",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "Point",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "ProgramTypeId",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "WithoutFee",
                table: "Programs");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Programs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Programs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
