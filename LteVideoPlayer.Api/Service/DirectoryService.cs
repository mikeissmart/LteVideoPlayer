using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Entities;
using LteVideoPlayer.Api.Persistance;
using Microsoft.EntityFrameworkCore;

namespace LteVideoPlayer.Api.Service
{
    public interface IDirectoryService
    {
        bool FileExists(FileDto file, bool isStaging);
        DirsAndFilesDto GetDirsAndFiles(string dirPathName, bool isStaging);
        FileDto? GetNextFile(FileDto file, bool isStaging);
    }

    public class DirectoryService : IDirectoryService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<DirectoryService> _logger;
        private readonly VideoConfig _videoConfig;

        public DirectoryService(AppDbContext appDbContext,
            ILogger<DirectoryService> logger,
            VideoConfig videoConfig)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _videoConfig = videoConfig;
        }

        public bool FileExists(FileDto file, bool isStaging)
        {
            return File.Exists((isStaging ? _videoConfig.RootPath : _videoConfig.StagePath) + file.FilePathName);
        }

        public DirsAndFilesDto GetDirsAndFiles(string dirPathName, bool isStaging)
        {
            try
            {
                var files = GetFiles(dirPathName, isStaging);
                var dirs = GetDirs(dirPathName, isStaging);
                if (isStaging)
                {
                    var a = _appDbContext.ConvertFiles.ToList();
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
                var rootPath = isStaging ? _videoConfig.StagePath : _videoConfig.RootPath;
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

        private List<DirDto> GetDirs(string dir, bool isStaging)
        {
            var rootPath = isStaging ? _videoConfig.StagePath : _videoConfig.RootPath;
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
            var rootPath = isStaging ? _videoConfig.StagePath : _videoConfig.RootPath;
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
