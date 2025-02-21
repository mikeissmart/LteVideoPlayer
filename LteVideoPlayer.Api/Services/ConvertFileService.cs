using AutoMapper;
using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.CronJobs;
using LteVideoPlayer.Api.Helpers;
using LteVideoPlayer.Api.Models.Dtos;
using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Models.Enums;
using LteVideoPlayer.Api.Persistance;
using LteVideoPlayer.Api.Persistance.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;

namespace LteVideoPlayer.Api.Services
{
    public interface IConvertFileService : IService
    {
        Task<List<ConvertFileDto>> GetAllConvertFilesAsync(DirectoryEnum dirEnum);
        Task<List<ConvertFileDto>> GetAllIncompleteConvertFilesAsync(DirectoryEnum dirEnum);
        Task<List<ConvertFileDto>> GetAllCompletedConvertFilesAsync(DirectoryEnum dirEnum);
        Task<List<ConvertFileDto>> GetConvertFileByOriginalFileAsync(DirectoryEnum dirEnum, FileDto file);
        Task<ConvertFileDto?> GetConvertFileByIdAsync(DirectoryEnum dirEnum, int id);
        Task<ConvertFileDto> AddConvertFileAsync(DirectoryEnum dirEnum, CreateConvertDto convert, ModelStateDictionary? modelState = null);
        Task<ConvertManyFileDto> AddConvertManyFileAsync(DirectoryEnum dirEnum, CreateManyConvertDto convert, ModelStateDictionary? modelState = null);
        Task<ConvertFileDto> UpdateConvertAsync(ConvertFileDto convert);
        Task DeleteConvertAsync(ConvertFileDto convert);
        Task<MetadataDto> GetVideoMetaAsync(DirectoryEnum dirEnum, string fileFullPath);
        ConvertFileDto? GetCurrentConvertFile();
    }

    public class ConvertFileService : BaseService, IConvertFileService
    {
        private readonly ILogger<ConvertFileService> _logger;
        private readonly IMapper _mapper;
        private readonly FfmegConfig _ffmegConfig;
        private readonly IVideoConfigService _videoConfigService;
        private readonly IConvertFileRepository _convertFileRepository;
        private readonly IDirectoryService _directoryService;
        private readonly ConvertCronJobService _convertCronJobService;

        public ConvertFileService(ILogger<ConvertFileService> logger, IMapper mapper, FfmegConfig ffmegConfig, IVideoConfigService videoConfigService, IConvertFileRepository convertFileRepository, IDirectoryService directoryService, ConvertCronJobService convertCronJobService)
        {
            _logger = logger;
            _mapper = mapper;
            _ffmegConfig = ffmegConfig;
            _videoConfigService = videoConfigService;
            _convertFileRepository = convertFileRepository;
            _directoryService = directoryService;
            _convertCronJobService = convertCronJobService;
        }

        public async Task<List<ConvertFileDto>> GetAllConvertFilesAsync(DirectoryEnum dirEnum)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanConvertVideo)
                    throw new Exception("Convert video disabled for this directory");

                var convertFiles = await _convertFileRepository.GetAllIncompleteConvertFilessAsync(dirEnum, false);
                var dtos = _mapper.Map<List<ConvertFileDto>>(convertFiles);

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<List<ConvertFileDto>> GetAllIncompleteConvertFilesAsync(DirectoryEnum dirEnum)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanConvertVideo)
                    throw new Exception("Convert video disabled for this directory");

                var convertFiles = await _convertFileRepository.GetAllIncompleteConvertFilessAsync(dirEnum, false);
                var dtos = _mapper.Map<List<ConvertFileDto>>(convertFiles);

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<List<ConvertFileDto>> GetAllCompletedConvertFilesAsync(DirectoryEnum dirEnum)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanConvertVideo)
                    throw new Exception("Convert video disabled for this directory");

                var convertFiles = await _convertFileRepository.GetAllConvertFilesAsync(dirEnum, videoConfig.RootVideoDir);
                var dtos = _mapper.Map<List<ConvertFileDto>>(convertFiles);

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<List<ConvertFileDto>> GetConvertFileByOriginalFileAsync(DirectoryEnum dirEnum, FileDto file)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanConvertVideo)
                    throw new Exception("Convert video disabled for this directory");

                return _mapper.Map<List<ConvertFileDto>>(await _convertFileRepository.GetConvertFileByOriginalAsync(dirEnum, file.Path, file.File));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ConvertFileDto?> GetConvertFileByIdAsync(DirectoryEnum dirEnum, int id)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanConvertVideo)
                    throw new Exception("Convert video disabled for this directory");

                ConvertFileDto? dto = null;
                var convertFile = await _convertFileRepository.GetConvertFileByIdAsync(id);
                if (convertFile != null)
                    dto = _mapper.Map<ConvertFileDto>(convertFile);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<ConvertFileDto> AddConvertFileAsync(DirectoryEnum dirEnum, CreateConvertDto convert, ModelStateDictionary? modelState = null)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanConvertVideo)
                    throw new Exception("Convert video disabled for this directory");

                var entity = await ConvertAsync(dirEnum, videoConfig, convert, modelState);
                await _convertFileRepository.AddAsync(entity);
                await _convertFileRepository.SaveChangesAsync();

                return _mapper.Map<ConvertFileDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<ConvertManyFileDto> AddConvertManyFileAsync(DirectoryEnum dirEnum, CreateManyConvertDto convert, ModelStateDictionary? modelState = null)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanConvertVideo)
                    throw new Exception("Convert video disabled for this directory");

                var result = new ConvertManyFileDto();
                foreach (var item in convert.Converts)
                {
                    result.Converts.Add(_mapper.Map<ConvertFileDto>(await ConvertAsync(dirEnum, videoConfig, item, modelState)));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

        }

        public async Task<ConvertFileDto> UpdateConvertAsync(ConvertFileDto convert)
        {
            try
            {
                var entity = _mapper.Map<ConvertFile>(convert);
                _convertFileRepository.Update(entity);
                await _convertFileRepository.SaveChangesAsync();
                _convertFileRepository.Detach(entity);

                return convert;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task DeleteConvertAsync(ConvertFileDto convert)
        {
            try
            {
                var entity = _mapper.Map<ConvertFile>(convert);
                _convertFileRepository.Remove(entity);
                await _convertFileRepository.SaveChangesAsync();
                _convertFileRepository.Detach(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<MetadataDto> GetVideoMetaAsync(DirectoryEnum dirEnum, string fullPath)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanConvertVideo)
                    throw new Exception("Convert video disabled for this directory");

                var file = Path.Combine(videoConfig.RootVideoDir, fullPath);
                var result = await ProcessHelper.RunProcessAsync(
                    _ffmegConfig.RootFfprobeFile,
                    $@"-hide_banner -i ""{file}""");

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

                return new MetadataDto
                {
                    Output = result.Output,
                    Error = result.Error,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public ConvertFileDto? GetCurrentConvertFile()
        {
            return _convertCronJobService.CurrentConvertFile();
        }

        private async Task<ConvertFile> ConvertAsync(DirectoryEnum dirEnum, IVideoConfig videoConfig, CreateConvertDto dto, ModelStateDictionary? modelState = null)
        {
            var stage = "";
            var error = "";

            if (string.IsNullOrEmpty(dto.OriginalFile.Path))
            {
                stage = "OriginalFile.Path";
                error = "Must have OriginalFile.Path";
            }
            if (string.IsNullOrEmpty(dto.OriginalFile.File))
            {
                stage = "OriginalFile.File";
                error = "Must have OriginalFile.File";
            }
            if (string.IsNullOrEmpty(dto.ConvertedFile.Path))
            {
                stage = "ConvertedFile.Path";
                error = "Must have ConvertedFile.Path";
            }
            if (string.IsNullOrEmpty(dto.ConvertedFile.File))
            {
                stage = "ConvertedFile.File";
                error = "Must have ConvertedFile.File";
            }

            var convertFile = await _convertFileRepository.GetConvertFileByOriginalAsync(dirEnum, dto.OriginalFile.Path, dto.OriginalFile.File);
            if (convertFile != null && convertFile.EndedDate == null)
            {
                stage = "ConvertedFile";
                error = "File already queued to be converted";
            }

            if (stage != "")
            {
                modelState?.AddModelError($"{stage}: {dto.OriginalFile.FullPath}", error);
                throw new ArgumentException(error);
            }

            return _mapper.Map<ConvertFile>(dto);
        }
    }
}
