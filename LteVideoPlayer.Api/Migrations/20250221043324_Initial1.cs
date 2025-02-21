using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LteVideoPlayer.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "UserProfiles");

            migrationBuilder.AddColumn<int>(
                name: "DirectoryEnum",
                table: "ThumbnailErrors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DirectoryEnum",
                table: "ConvertFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DirectoryEnum",
                table: "ThumbnailErrors");

            migrationBuilder.DropColumn(
                name: "DirectoryEnum",
                table: "ConvertFiles");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
