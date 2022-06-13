using AutoMapper;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Entities;
using LteVideoPlayer.Api.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LteVideoPlayer.Api.Service
{
    public interface IFileHistoryService
    {
        Task<List<FileHistoryDto>> GetFileHistoriesByUserProfileAsync(int userProfileId);
        Task<FileHistoryDto> AddUpdateFileHistoryAsync(FileHistoryDto fileHistory);
    }

    public class FileHistoryService : IFileHistoryService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<FileHistoryService> _logger;
        private readonly IDirectoryService _directoryService;
        private readonly IMapper _mapper;

        public FileHistoryService(AppDbContext appDbContext,
            ILogger<FileHistoryService> logger,
            IDirectoryService directoryService,
            IMapper mapper)
        {
            _appDbContext = appDbContext;
            _directoryService = directoryService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<FileHistoryDto>> GetFileHistoriesByUserProfileAsync(int userProfileId)
        {
            try
            {
                var histories = _mapper.Map<List<FileHistoryDto>>(await _appDbContext.FileHistories
                    .Where(x => x.UserProfileId == userProfileId)
                    .AsNoTracking()
                    .ToListAsync());
                foreach (var history in histories)
                    history.FileEntity.FileExists = _directoryService.FileExists(history.FileEntity, false);

                return histories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<FileHistoryDto> AddUpdateFileHistoryAsync(FileHistoryDto fileHistory)
        {
            try
            {
                var file = _mapper.Map<FileHistory>(fileHistory);
                var entity = fileHistory.Id == 0
                    ? await _appDbContext.FileHistories.AddAsync(file)
                    : _appDbContext.FileHistories.Update(file);
                await _appDbContext.SaveChangesAsync();

                return _mapper.Map<FileHistoryDto>(entity.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
