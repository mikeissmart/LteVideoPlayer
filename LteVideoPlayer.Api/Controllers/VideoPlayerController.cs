using LteVideoPlayer.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LteVideoPlayer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoPlayerController : ControllerBase
    {
        private const string _rootPath = @"\\192.168.1.200\Share\Videos";

        [HttpGet("GetSeriesAndSeasons")]
        public IActionResult GetSeriesAndSeasons()
        {
            var dirs = GetDirAndFiles(_rootPath, null);
            return Ok(dirs);
        }

        [HttpGet("StreamVideo")]
        public FileResult StreamVideo([FromQuery] string fileDir)
        {
            var pathDir = _rootPath + fileDir;
            return new FileStreamResult(
                new FileStream(pathDir, FileMode.Open,
                    FileAccess.Read, FileShare.Read),
                "application/octet-stream")
            {
                EnableRangeProcessing = true
            };
        }

        private List<DirDto> GetDirAndFiles(string path, string? dirName)
        {
            var dirRootPath = path +
                (dirName != null ? ("\\" + dirName) : "");
            var dirPath = dirRootPath.Replace(_rootPath, "");
            var dirs = Directory.GetDirectories(dirRootPath)
                .OrderBy(x => x)
                .Select(x => new DirDto
                {
                    Path = dirPath,
                    Name = x.Replace(dirRootPath + "\\", "")
                })
                .ToList();
            foreach (var dir in dirs)
            {
                dir.SubDirs = GetDirAndFiles(dirRootPath,  dir.Name);
                dir.Videos = Directory.GetFiles(dirRootPath + "\\" + dir.Name)
                    .OrderBy(x => x)
                    .Select(x => x.Replace(dirRootPath + "\\" + dir.Name + "\\", ""))
                    .ToList();
            }

            return dirs;
        }
    }
}
