using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LteVideoPlayer.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConvertFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Output = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Errored = table.Column<bool>(type: "bit", nullable: false),
                    OriginalFile_Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalFile_File = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConvertedFile_Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConvertedFile_File = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AudioStreamIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvertFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThumbnailErrors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimesFailed = table.Column<int>(type: "int", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    File_Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    File_File = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastError = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThumbnailErrors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConvertFiles");

            migrationBuilder.DropTable(
                name: "ThumbnailErrors");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
