using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LteVideoPlayer.Api.Migrations.LogDb
{
    /// <inheritdoc />
    public partial class Update_CronLog_ErrorMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "CronLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErrorStackTrace",
                table: "CronLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Errored",
                table: "CronLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "CronLogs");

            migrationBuilder.DropColumn(
                name: "ErrorStackTrace",
                table: "CronLogs");

            migrationBuilder.DropColumn(
                name: "Errored",
                table: "CronLogs");
        }
    }
}
