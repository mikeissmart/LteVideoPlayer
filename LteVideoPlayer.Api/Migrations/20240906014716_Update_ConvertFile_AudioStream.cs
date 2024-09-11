using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LteVideoPlayer.Api.Migrations
{
    public partial class Update_ConvertFile_AudioStream : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AudioStream",
                table: "ConvertFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioStream",
                table: "ConvertFiles");
        }
    }
}
