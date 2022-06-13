using AutoMapper;
using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Entities;
using LteVideoPlayer.Api.Persistance;
using Microsoft.EntityFrameworkCore;

namespace LteVideoPlayer.Api.Service
{
    /*public interface IVideoFileService
    {
        Task<VideoDirsAndFilesDto> GetAllVideoDirsAndFilesAsync();
        Task<FileDto?> GetVideoFilesByIdAsync(int videoFileId);
        Task<FileDto> UpdateVideoFileAsync(FileDto videoFile);
    }

    public class VideoFileService : IVideoFileService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<VideoFileService> _logger;
        private readonly VideoConfig _videoConfig;

        public VideoFileService(AppDbContext appDbContext,
            IMapper mapper,
            ILogger<VideoFileService> logger,
            VideoConfig videoConfig)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _logger = logger;
            _videoConfig = videoConfig;
        }

        public async Task<VideoDirsAndFilesDto> GetAllVideoDirsAndFilesAsync()
        {
            try
            {
                var videoFiles = _mapper.Map<List<FileDto>>(await UpdateAllVideoFilesAsync());
                var dirs = videoFiles
                    .Select(x => x.VideoPath)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
                return new VideoDirsAndFilesDto
                {
                    Dirs = dirs,
                    Files = videoFiles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<FileDto?> GetVideoFilesByIdAsync(int videoFileId)
        {
            try
            {
                var videoFile = (await UpdateAllVideoFilesAsync())
                    .Where(x => x.Id == videoFileId)
                    .SingleOrDefault();

                if (videoFile != null)
                    return _mapper.Map<FileDto>(videoFile);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<FileDto> UpdateVideoFileAsync(FileDto videoFile)
        {
            try
            {
                var video = _mapper.Map<VideoFile>(videoFile);
                _appDbContext.Entry(video).State = EntityState.Modified;
                await _appDbContext.SaveChangesAsync();
                _appDbContext.Entry(video).State = EntityState.Detached;

                return _mapper.Map<FileDto>(await _appDbContext.VideoFiles
                    .Where(x => x.Id == videoFile.Id)
                    .AsNoTracking()
                    .FirstAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private async Task<List<VideoFile>> UpdateAllVideoFilesAsync()
        {
            var existingVideoFiles = await _appDbContext.VideoFiles
                .AsNoTracking()
                .ToListAsync();
            var systemVideoFiles = GetVideoFiles("", await _appDbContext.NotVideoExts
                .AsNoTracking()
                .ToListAsync());

            // Un/Delete
            var deletedVideos = new List<VideoFile>();
            foreach (var videoFile in existingVideoFiles)
            {
                var checkFile = systemVideoFiles
                    .SingleOrDefault(x => x.VideoPath == videoFile.VideoPath &&
                        x.VideoName == videoFile.VideoName);
                if (checkFile == null)
                {
                    deletedVideos.Add(videoFile);
                    if (videoFile.DeletedDate != null)
                    {
                        videoFile.DeletedDate = DateTime.Now;
                        _appDbContext.VideoFiles.Update(videoFile);
                    }
                }
                else if (videoFile.DeletedDate != null)
                {
                    videoFile.DeletedDate = null;
                    _appDbContext.VideoFiles.Update(videoFile);
                }
            }
            foreach (var videoFile in deletedVideos)
                existingVideoFiles.Remove(videoFile);

            // Add
            var addVideos = new List<VideoFile>();
            foreach (var videoFile in systemVideoFiles)
            {
                var checkFile = existingVideoFiles
                    .FirstOrDefault(x => x.VideoPath == videoFile.VideoPath &&
                        x.VideoName == videoFile.VideoName);
                if (checkFile == null)
                {
                    var entity = await _appDbContext.AddAsync(videoFile);
                    try
                    {
                        await _appDbContext.SaveChangesAsync();
                        addVideos.Add(entity.Entity);
                        entity.State = EntityState.Detached;
                    }
                    catch
                    {
                        // Might attempt to add same video file more than once
                        //  Since this is called so much
                    }
                }
            }
            existingVideoFiles.AddRange(addVideos);

            return existingVideoFiles
                .OrderBy(x => x.VideoPath)
                    .ThenBy(x => x.VideoName)
                .ToList();
        }

        private List<VideoFile> GetVideoFiles(string path, List<NotVideoExt> notVideoExts)
        {
            var files = Directory.GetFiles(_videoConfig.RootPath + path)
                .OrderBy(x => x)
                .ToList()
                .Select(x => new VideoFile
                {
                    VideoPath = x.Replace(Path.GetFileName(x), "").Replace(_videoConfig.RootPath, ""),
                    VideoName = Path.GetFileName(x)
                })
                .ToList();
            for (var i = 0; i < files.Count; i++)
            {
                var ext = Path.GetExtension(files[i].VideoName);
                if (notVideoExts.Any(x => x.Ext == ext))
                {
                    files.Remove(files[i]);
                    i--;
                }
            }

            foreach (var dir in Directory.GetDirectories(_videoConfig.RootPath + path).OrderBy(x => x))
                files.AddRange(GetVideoFiles(dir.Replace(_videoConfig.RootPath, ""), notVideoExts));

            return files;
        }
    }*/
}
