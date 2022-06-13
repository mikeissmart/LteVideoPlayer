using LteVideoPlayer.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LteVideoPlayer.Api.Controllers
{
    /*[ApiController]
    [Route("api/[controller]")]
    public class VideoPlayerController : ControllerBase
    {
        private const string _rootPath = @"\\192.168.1.200\Share\Videos";
        private const string _ffmpegFile = @"\\192.168.1.200\Share\ffmpeg\ffmpeg.exe";

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

        [HttpPost("ConvertFile")]
        public async Task<IActionResult> ConvertFile([FromBody] FileDto file)
        {
            var convertFile = file.Path.Remove(file.Path.Length - Path.GetExtension(file.Path).Length);
            convertFile += ".mp4";

            if (System.IO.File.Exists(_rootPath + convertFile))
                System.IO.File.Delete(_rootPath + convertFile);

            var error = "";
            var output = "";
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegFile,
                    Arguments = $@"-i ""{_rootPath + file.Path}"" -vcodec libx264 -acodec aac -y ""{_rootPath + convertFile}""",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                using (var exeProcess = Process.Start(startInfo))
                {
                    error = exeProcess!.StandardError.ReadToEnd();
                    output = exeProcess!.StandardError.ReadToEnd();
                    await exeProcess.WaitForExitAsync();
                }

                //System.IO.File.Delete(_rootPath + file.Path);

                /*file.Path = convertFile;
                file.Name = Path.GetFileName(file.Path);* /

                return Ok(file);
            }
            catch (Exception ex)
            {
                return Ok(null);
            }
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
                dir.SubDirs = GetDirAndFiles(dirRootPath, dir.Name);
                dir.Videos = Directory.GetFiles(dirRootPath + "\\" + dir.Name)
                    .OrderBy(x => x)
                    .Select(x => new FileDto
                    {
                        Path = x.Replace(_rootPath, ""),
                        Name = x.Replace(dirRootPath + "\\" + dir.Name + "\\", "")
                    })
                    .ToList();
            }

            return dirs;
        }
    }*/
}
