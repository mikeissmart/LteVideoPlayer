using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LteVideoPlayer.Api.Migrations
{
    public partial class Add_Initial : Migration
    {
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
                    OriginalFile_FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalFile_FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConvertedFile_FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConvertedFile_FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvertFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserProfileId = table.Column<int>(type: "int", nullable: false),
                    PercentWatched = table.Column<float>(type: "real", nullable: false),
                    FileEntity_FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileEntity_FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileHistories_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileHistories_UserProfileId",
                table: "FileHistories",
                column: "UserProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConvertFiles");

            migrationBuilder.DropTable(
                name: "FileHistories");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
