using AutoMapper;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Entities;
using LteVideoPlayer.Api.Persistance;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace LteVideoPlayer.Api.Service
{
    public interface IUserProfileService
    {
        Task<List<UserProfileDto>> GetAllUserProfilesAsync();
        Task<UserProfileDto?> GetUserProfileByIdAsync(int userProfileId);
        Task<UserProfileDto> CreateUserProfileAsync(UserProfileDto userProfile, ModelStateDictionary? modelState = null);
        Task<UserProfileDto> UpdateUserProfileAsync(UserProfileDto userProfile, ModelStateDictionary? modelState = null);
        Task DeleteUserProfileAsync(int userProfileId);
    }

    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(AppDbContext appDbContext,
            IMapper mapper,
            ILogger<UserProfileService> logger)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<UserProfileDto>> GetAllUserProfilesAsync()
        {
            try
            {
                return _mapper.Map<List<UserProfileDto>>(await _appDbContext.UserProfiles
                    .Where(x => x.DeletedDate == null)
                    .AsNoTracking()
                    .ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<UserProfileDto?> GetUserProfileByIdAsync(int userProfileId)
        {
            try
            {
                var profile = await _appDbContext.UserProfiles
                    .Where(x => x.DeletedDate == null && x.Id == userProfileId)
                    .AsNoTracking()
                    .SingleOrDefaultAsync();

                return profile != null
                    ? _mapper.Map<UserProfileDto>(profile)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<UserProfileDto> CreateUserProfileAsync(UserProfileDto userProfile, ModelStateDictionary? modelState = null)
        {
            try
            {
                var profile = _mapper.Map<UserProfile>(userProfile);

                if (_appDbContext.UserProfiles.Any(x => x.DeletedDate == null &&
                    x.Name == userProfile.Name))
                {
                    var error = "User profile with name already exists";
                    if (modelState != null)
                        modelState.AddModelError(nameof(UserProfileDto.Name), error);
                    throw new ArgumentException(error);
                }
                if (userProfile.Name == "New Profile")
                {
                    var error = "Cannot use New Profile as Name";
                    if (modelState != null)
                        modelState.AddModelError(nameof(UserProfileDto.Name), error);
                    throw new ArgumentException(error);
                }

                var entity = await _appDbContext.UserProfiles.AddAsync(profile);
                await _appDbContext.SaveChangesAsync();

                return _mapper.Map<UserProfileDto>(entity.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<UserProfileDto> UpdateUserProfileAsync(UserProfileDto userProfile, ModelStateDictionary? modelState = null)
        {
            try
            {
                if (_appDbContext.UserProfiles.Any(x => x.Name == userProfile.Name && x.Id != userProfile.Id))
                {
                    var error = "User profile with name already exists";
                    if (modelState != null)
                        modelState.AddModelError(nameof(UserProfileDto.Name), error);
                    throw new ArgumentException(error);
                }
                if (userProfile.Name == "New Profile")
                {
                    var error = "Cannot use New Profile as Name";
                    if (modelState != null)
                        modelState.AddModelError(nameof(UserProfileDto.Name), error);
                    throw new ArgumentException(error);
                }

                _appDbContext.Entry(_mapper.Map<UserProfile>(userProfile)).State = EntityState.Modified;
                await _appDbContext.SaveChangesAsync();

                return (await GetUserProfileByIdAsync(userProfile.Id))!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteUserProfileAsync(int userProfileId)
        {
            try
            {
                var profile = await _appDbContext.UserProfiles
                    .Where(x => x.DeletedDate == null && x.Id == userProfileId)
                    .SingleOrDefaultAsync();

                if (profile != null)
                {
                    profile.DeletedDate = DateTime.Now;
                    await _appDbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
