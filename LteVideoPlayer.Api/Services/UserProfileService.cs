using AutoMapper;
using LteVideoPlayer.Api.Models.Dtos;
using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Persistance;
using LteVideoPlayer.Api.Persistance.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LteVideoPlayer.Api.Services
{
    public interface IUserProfileService : IService
    {
        Task<List<UserProfileDto>> GetAllUserProfilesAsync();
        Task<UserProfileDto?> GetUserProfileByIdAsync(int id);
        Task<UserProfileDto> CreateUserProfileAsync(UserProfileDto userProfile, ModelStateDictionary? modelState = null);
        Task<UserProfileDto> UpdateUserProfileAsync(UserProfileDto userProfile, ModelStateDictionary? modelState = null);
        Task DeleteUserProfileAsync(int id);
    }

    public class UserProfileService : BaseService, IUserProfileService
    {
        private readonly ILogger<UserProfileService> _logger;
        private readonly IMapper _mapper;
        private readonly IUserProfileRepository _userProfileRepository;

        public UserProfileService(ILogger<UserProfileService> logger, IMapper mapper, IUserProfileRepository userProfileRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _userProfileRepository = userProfileRepository;
        }

        public async Task<List<UserProfileDto>> GetAllUserProfilesAsync()
        {
            try
            {
                return _mapper.Map<List<UserProfileDto>>(await _userProfileRepository.GetAllUserProfilesAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<UserProfileDto?> GetUserProfileByIdAsync(int id)
        {
            try
            {
                var entity = await _userProfileRepository.GetUserProfileByIdAsync(id);
                return entity != null
                    ? _mapper.Map<UserProfileDto>(entity)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<UserProfileDto> CreateUserProfileAsync(UserProfileDto userProfile, ModelStateDictionary? modelState = null)
        {
            try
            {
                if (await _userProfileRepository.GetUserProfileByNameAsync(userProfile.Name) == null)
                {
                    var error = "User profile with name already exists";
                    if (modelState != null)
                        modelState.AddModelError(nameof(UserProfileDto.Name), error);
                    throw new ArgumentException(error);
                }

                var entity = await _userProfileRepository.AddAsync(_mapper.Map<UserProfile>(userProfile));
                await _userProfileRepository.SaveChangesAsync();

                return _mapper.Map<UserProfileDto>(entity.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<UserProfileDto> UpdateUserProfileAsync(UserProfileDto userProfile, ModelStateDictionary? modelState = null)
        {
            try
            {
                var check = await _userProfileRepository.GetUserProfileByNameAsync(userProfile.Name);
                if (check != null && check.Id != userProfile.Id)
                {
                    var error = "User profile with name already exists";
                    if (modelState != null)
                        modelState.AddModelError(nameof(UserProfileDto.Name), error);
                    throw new ArgumentException(error);
                }

                _userProfileRepository.ClearTracking();
                var entity = _mapper.Map<UserProfile>(userProfile);
                _userProfileRepository.Update(entity);
                await _userProfileRepository.SaveChangesAsync();

                return _mapper.Map<UserProfileDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task DeleteUserProfileAsync(int id)
        {
            try
            {
                var entity = await _userProfileRepository.GetUserProfileByIdAsync(id);
                if (entity != null)
                {
                    _userProfileRepository.Remove(entity);
                    await _userProfileRepository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

    }
}
