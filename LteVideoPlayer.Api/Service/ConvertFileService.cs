using AutoMapper;
using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.CronJob.Convert;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Entities;
using LteVideoPlayer.Api.Persistance;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;

namespace LteVideoPlayer.Api.Service
{
    public interface IConvertFileService
    {
        Task<List<ConvertFileDto>> GetIncompleteConvertsAsync(int maxCount = -1);
        Task<List<ConvertFileDto>> GetAllConvertFilesAsync();
        Task<List<ConvertFileDto>> GetConvertFileByOriginalFileAsync(FileDto file);
        Task<ConvertFileDto> GetConvertFileByIdAsync(int convertFileId);
        Task<ConvertFileDto> AddConvertFileAsync(CreateConvertDto convert, ModelStateDictionary? modelState = null);
        Task<ConvertManyFileDto> AddConvertManyFileAsync(CreateManyConvertDto convert, ModelStateDictionary? modelState = null);
        Task<ConvertFileDto> UpdateConvertAsync(ConvertFileDto convert);
        Task DeleteConvertAsync(ConvertFileDto convert);
        List<ConvertFileDto> WorkingConvertFiles();
    }

    public class ConvertFileService : IConvertFileService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<ConvertFileService> _logger;
        private readonly IDirectoryService _directoryService;
        private readonly IMapper _mapper;
        private readonly ConvertQueueCronJob _convertQueueCronJob;

        public ConvertFileService(AppDbContext appDbContext,
            ILogger<ConvertFileService> logger,
            IDirectoryService directoryService,
            IMapper mapper,
            ConvertQueueCronJob convertQueueCronJob)
        {
            _appDbContext = appDbContext;
            _directoryService = directoryService;
            _logger = logger;
            _mapper = mapper;
            _convertQueueCronJob = convertQueueCronJob;
        }

        public async Task<List<ConvertFileDto>> GetIncompleteConvertsAsync(int maxCount = -1)
        {
            try
            {
                var query = _appDbContext.ConvertFiles
                    .Where(x => x.EndedDate == null)
                    .OrderBy(x => x.CreatedDate)
                    .AsNoTracking();
                if (maxCount > -1)
                    query = query.Take(maxCount);

                var convertFiles = _mapper.Map<List<ConvertFileDto>>(await query.ToListAsync());
                CheckFileExists(convertFiles);

                return convertFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<ConvertFileDto>> GetAllConvertFilesAsync()
        {
            var convertFiles = _mapper.Map<List<ConvertFileDto>>(await _appDbContext.ConvertFiles
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking()
                .ToListAsync());
            CheckFileExists(convertFiles);

            return convertFiles;
        }

        public async Task<List<ConvertFileDto>> GetConvertFileByOriginalFileAsync(FileDto file)
        {
            try
            {
                var convertFiles = _mapper.Map<List<ConvertFileDto>>(await _appDbContext.ConvertFiles
                    .Where(x => x.OriginalFile.FilePath == file.FilePath &&
                        x.OriginalFile.FileName == file.FileName)
                    .OrderBy(x => x.CreatedDate)
                    .AsNoTracking()
                    .ToListAsync());
                CheckFileExists(convertFiles);

                return convertFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ConvertFileDto> GetConvertFileByIdAsync(int convertFileId)
        {
            try
            {
                var convertFile = _mapper.Map<ConvertFileDto>(await _appDbContext.ConvertFiles
                    .Where(x => x.Id == convertFileId)
                    .OrderBy(x => x.CreatedDate)
                    .AsNoTracking()
                    .SingleOrDefaultAsync());
                CheckFileExists(new[] { convertFile });

                return convertFile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ConvertFileDto> AddConvertFileAsync(CreateConvertDto convert, ModelStateDictionary? modelState = null)
        {
            try
            {
                return await ConvertAsync(convert, modelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ConvertManyFileDto> AddConvertManyFileAsync(CreateManyConvertDto convert, ModelStateDictionary? modelState = null)
        {
            var result = new ConvertManyFileDto();
            foreach (var item in convert.Converts)
            {
                try
                {
                    result.Converts.Add(await ConvertAsync(item));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

            return result;
        }

        public async Task<ConvertFileDto> UpdateConvertAsync(ConvertFileDto convert)
        {
            var entity = _mapper.Map<ConvertFile>(convert);
            _appDbContext.Entry(entity).State = EntityState.Modified;
            await _appDbContext.SaveChangesAsync();
            _appDbContext.Entry(entity).State = EntityState.Detached;

            return convert;
        }

        public List<ConvertFileDto> WorkingConvertFiles()
        {
            return _convertQueueCronJob.CurrentConverts();
        }

        private async Task<ConvertFileDto> ConvertAsync(CreateConvertDto convert, ModelStateDictionary? modelState = null)
        {
            var stage = "";
            var error = "";

            if (string.IsNullOrEmpty(convert.OriginalFile.FilePath))
            {
                stage = "OriginalFile.FilePath";
                error = "Must have OriginalFile.FilePath";
            }
            if (string.IsNullOrEmpty(convert.OriginalFile.FileName))
            {
                stage = "OriginalFile.FileName";
                error = "Must have OriginalFile.FileName";
            }
            if (string.IsNullOrEmpty(convert.ConvertedFile.FilePath))
            {
                stage = "ConvertedFile.FilePath";
                error = "Must have ConvertedFile.FilePath";
            }
            if (string.IsNullOrEmpty(convert.ConvertedFile.FileName))
            {
                stage = "ConvertedFile.FileName";
                error = "Must have ConvertedFile.FileName";
            }

            var entity = _mapper.Map<ConvertFile>(convert);
            if (_appDbContext.ConvertFiles.Any(x => x.EndedDate == null &&
                x.OriginalFile.FileName == entity.OriginalFile.FileName &&
                x.OriginalFile.FilePath == entity.OriginalFile.FilePath))
            {
                stage = "ConvertedFile";
                error = "File already queued to be converted";
            }

            if (stage != "")
            {
                modelState?.AddModelError($"{stage}: {entity.OriginalFile.FileName}", error);
                throw new ArgumentException(error);
            }

            var lastConvert = _appDbContext.ConvertFiles
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault();

            if (lastConvert != null)
            {
                if (lastConvert.CreatedDate.AddSeconds(1) > entity.CreatedDate)
                    entity.CreatedDate = lastConvert.CreatedDate.AddSeconds(1);
            }

            await _appDbContext.ConvertFiles.AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
            _appDbContext.Entry(entity).State = EntityState.Detached;

            return await GetConvertFileByIdAsync(entity.Id);
        }

        public async Task DeleteConvertAsync(ConvertFileDto convert)
        {
            var entity = _mapper.Map<ConvertFile>(convert);
            _appDbContext.Entry(entity).State = EntityState.Deleted;
            await _appDbContext.SaveChangesAsync();
            _appDbContext.Entry(entity).State = EntityState.Detached;
        }

        private void CheckFileExists(IEnumerable<ConvertFileDto> convertFiles)
        {
            foreach (var file in convertFiles)
            {
                file.OriginalFile.FileExists = _directoryService.FileExists(file.OriginalFile, false);
                file.ConvertedFile.FileExists = _directoryService.FileExists(file.ConvertedFile, true);
            }
        }
    }
}
