using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.CronJob.Convert;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Entities;
using LteVideoPlayer.Api.Helpers;
using LteVideoPlayer.Api.Persistance;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace LteVideoPlayer.Api.Service
{
    public interface IDirectoryService
    {
        bool FileExists(FileDto file, bool isStaging);
        DirsAndFilesDto GetDirsAndFiles(string dirPathName, bool isStaging);
        FileDto? GetNextFile(FileDto file, bool isStaging);
        string GetFolderThumbnail(string filePathName);
        string GetFileThumbnail(string filePathName);
        void DeleteThumbnail(string filePathName);
        MetaDataDto GetVideoMeta(string filePathName, bool isStaging);
        string GetWorkingThumbnail();
    }

    public class DirectoryService : IDirectoryService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<DirectoryService> _logger;
        private readonly VideoConfig _videoConfig;
        private readonly ThumbnailCronJob _thumbnailCronJob;

        public DirectoryService(AppDbContext appDbContext,
            ILogger<DirectoryService> logger,
            VideoConfig videoConfig,
            ThumbnailCronJob thumbnailCronJob)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _videoConfig = videoConfig;
            _thumbnailCronJob = thumbnailCronJob;
        }

        public bool FileExists(FileDto file, bool isStaging)
        {
            return File.Exists((isStaging ? _videoConfig.VideoPath : _videoConfig.StagePath) + file.FilePathName);
        }

        public DirsAndFilesDto GetDirsAndFiles(string dirPathName, bool isStaging)
        {
            try
            {
                var files = GetFiles(dirPathName, isStaging);
                var dirs = GetDirs(dirPathName, isStaging);
                if (isStaging)
                {
                    for (var i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        if (file.FileName.Contains("Converting_"))
                        {
                            // Dont send converting files
                            files.RemoveAt(i--);
                        }
                        else
                        {
                            file.FileName = file.FileName.Replace("_converting", "");
                            file.ConvertQueued = _appDbContext.ConvertFiles.Any(x =>
                                x.EndedDate == null &&
                                x.OriginalFile.FilePath == file.FilePath &&
                                x.OriginalFile.FileName == file.FileName);
                        }
                    }
                }

                return new DirsAndFilesDto
                {
                    Dirs = dirs,
                    Files = files
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public FileDto? GetNextFile(FileDto file, bool isStaging)
        {
            var files = GetFiles(file.FilePath, isStaging);
            var index = -1;
            for (var i = 0; i < files.Count; i++)
            {
                if (files[i].FilePathName == file.FilePathName)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                throw new FileNotFoundException("File not found " + file.FilePathName);

            if (index == files.Count - 1)
            {
                // Last file goto next dir
                var rootPath = isStaging ? _videoConfig.StagePath : _videoConfig.VideoPath;
                var parentDir = Directory.GetParent(Path.Combine(
                    rootPath,
                    file.FilePath.Substring(0, file.FilePath.Length - 2)))!
                    .FullName.Replace(rootPath, "");
                var currentDirName = Directory.GetParent(Path.Combine(
                    rootPath,
                    file.FilePath))!
                    .Name + "\\";

                var dirs = GetDirs(parentDir + "\\", isStaging);
                index = -1;
                for (var i = 0; i < dirs.Count; i++)
                {
                    if (dirs[i].DirName == currentDirName)
                    {
                        index = i;
                        break;
                    }
                }

                // Last dir or dir not found?
                if (index == dirs.Count - 1 || index == -1)
                    return null;

                // Dir has no files
                files = GetFiles(dirs[index + 1].DirPathName, isStaging);
                if (files.Count == 0)
                    return null;

                // First file of next dir
                return files[0];
            }
            else
            {
                return files[index + 1];
            }
        }

        public string GetFolderThumbnail(string filePathName)
        {
            var thumbnailPath = Path.Combine(_videoConfig.ThumbnailPath, filePathName);
            if (!Directory.Exists(thumbnailPath))
                return "";

            var thumbnails = Directory.GetFiles(thumbnailPath)
                .OrderBy(x => x)
                .ToList();
            if (thumbnails.Count > 0)
                return thumbnails[0];

            foreach (var subpath in Directory.GetDirectories(thumbnailPath))
            {
                var tn = GetFolderThumbnail(subpath.Replace(_videoConfig.ThumbnailPath, ""));
                if (tn != "")
                    return tn;
            }

            return "";
        }

        public string GetFileThumbnail(string filePathName)
        {
            var path = Path.GetDirectoryName(filePathName);
            var thumbnail = Path.GetFileNameWithoutExtension(filePathName) + ".jpeg";
            var thumbnailPath = Path.Combine(path, thumbnail);

            thumbnailPath = Path.Combine(_videoConfig.ThumbnailPath, thumbnailPath);
            if (File.Exists(thumbnailPath))
                return thumbnailPath;

            return "";
        }

        public void DeleteThumbnail(string filePathName)
        {
            var path = Path.GetDirectoryName(filePathName);
            var thumbnail = Path.GetFileNameWithoutExtension(filePathName) + ".jpeg";
            var thumbnailPath = Path.Combine(path, thumbnail);

            thumbnailPath = Path.Combine(_videoConfig.ThumbnailPath, thumbnailPath);
            if (File.Exists(thumbnailPath))
                File.Delete(thumbnailPath);
        }

        public MetaDataDto GetVideoMeta(string filePathName, bool isStaging)
        {
            var rootPath = isStaging ? _videoConfig.StagePath : _videoConfig.VideoPath;
            var file = Path.Combine(rootPath, filePathName);

            ProcessHelper.RunProcess(
                _videoConfig.FfprobeFile,
                $@"-hide_banner -i ""{file}""",
                out var output,
                out var error);

            //$@"-v quiet -print_format json -show_format -show_streams ""{file}""",

            /*try
            {
                output = JToken.Parse(output).ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch { }
            try
            {
                error = JToken.Parse(error).ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch { }*/

            return new MetaDataDto
            {
                Output = output,
                Error = error
            };
        }

        public string GetWorkingThumbnail()
        {
            return _thumbnailCronJob.GetWorkingThunbmail();
        }

        private List<DirDto> GetDirs(string dir, bool isStaging)
        {
            var rootPath = isStaging ? _videoConfig.StagePath : _videoConfig.VideoPath;
            var dirs = Directory.GetDirectories(rootPath + dir)
                .OrderBy(x => x)
                .ToList()
                .Select(x => new DirDto
                {
                    DirPath = dir,
                    DirName = x.Replace(rootPath + dir, "") + "\\"
                })
                .ToList();

            return dirs;
        }

        private List<FileDto> GetFiles(string dir, bool isStaging)
        {
            var rootPath = isStaging ? _videoConfig.StagePath : _videoConfig.VideoPath;
            var files = Directory.GetFiles(rootPath + dir)
                .OrderBy(x => x)
                .ToList()
                .Select(x => new FileDto
                {
                    FilePath = x.Replace(Path.GetFileName(x), "").Replace(rootPath, ""),
                    FileName = Path.GetFileName(x),
                    FileExists = true
                })
                .ToList();

            return files;
        }
    }
}
