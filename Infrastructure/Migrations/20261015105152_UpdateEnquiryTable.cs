using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEnquiryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Enquiries");

            migrationBuilder.AddColumn<string>(
                name: "BestTime",
                table: "Enquiries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Program",
                table: "Enquiries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Qualification",
                table: "Enquiries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestTime",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "Program",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "Qualification",
                table: "Enquiries");

            migrationBuilder.AddColumn<string>(
                name: "AssignedToUserId",
                table: "Enquiries",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Enquiries",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
