using LteVideoPlayer.Api.Models.Entities;

namespace LteVideoPlayer.Api.Persistance.Repositories
{
    public interface IUserProfileRepository : IRepository<UserProfile>
    {
        Task<List<UserProfile>> GetAllUserProfilesAsync();
        Task<UserProfile?> GetUserProfileByIdAsync(int id);
        Task<UserProfile?> GetUserProfileByNameAsync(string name);
    }

    public class UserProfileRepository : BaseRepository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(AppDbContext dbContext): base(dbContext)
        {
            
        }

        public async Task<List<UserProfile>> GetAllUserProfilesAsync()
            => await GetAsync();

        public async Task<UserProfile?> GetUserProfileByIdAsync(int id)
            => await GetFirstOrDefaultAsync(x => x.Id == id);

        public async Task<UserProfile?> GetUserProfileByNameAsync(string name)
            => await GetFirstOrDefaultAsync(x => x.Name == name);
    }
}
