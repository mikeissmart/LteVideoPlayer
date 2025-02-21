using LteVideoPlayer.Api.Persistance;
using LteVideoPlayer.Api.Models;
using LteVideoPlayer.Api.Models.Dtos.Logging;
using LteVideoPlayer.Api.Models.Entities.Logging;
using Microsoft.EntityFrameworkCore;

namespace LteVideoPlayer.Api.Persistance.Repositories.Logging
{
    public interface IAppLogRepository : IRepository<AppLog>
    {
        Task<List<int>> AllEventIdsAsync();
        Task<Paginate<AppLog>> GetPaginatedLogsAsync(PaginateFilterDto<AppLogFilterDto> filter);
        Task<List<AppLog>> GetLogsBerforeDate(DateTime date);
    }

    public class AppLogRepository : BaseRepository<AppLog>, IAppLogRepository
    {
        public AppLogRepository(LogDbContext dbContext) : base(dbContext)
        { }

        public async Task<List<int>> AllEventIdsAsync() =>
            await GenerateQuery()
                .Select(x => x.EventId)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

        public async Task<Paginate<AppLog>> GetPaginatedLogsAsync(PaginateFilterDto<AppLogFilterDto> filter) =>
            await GetPaginateAsync(
                filter,
                predicate: x =>
                    (filter.Filter.LogLevel == null || x.LogLevel == filter.Filter.LogLevel) &&
                    (filter.Filter.EventId == null || x.EventId == filter.Filter.EventId) &&
                    (string.IsNullOrEmpty(filter.Filter.EventName) || x.EventName!.Contains(filter.Filter.EventName)) &&
                    (filter.Filter.MinDate == null || x.CreatedDateTime < filter.Filter.MinDate.Value) &&
                    (filter.Filter.MaxDate == null || x.CreatedDateTime >= filter.Filter.MaxDate.Value),
                orderBy: CalculateOrderBy(filter));

        public async Task<List<AppLog>> GetLogsBerforeDate(DateTime date) =>
            await GetAsync(x => x.CreatedDateTime < date);

        private Func<IQueryable<AppLog>, IOrderedQueryable<AppLog>>? CalculateOrderBy(PaginateFilterDto filter)
        {
            Func<IQueryable<AppLog>, IOrderedQueryable<AppLog>>? orderBy = null;
            switch (filter.OrderBy)
            {
                case nameof(AppLog.LogLevel):
                    orderBy = filter.IsOrderAsc
                        ? x => x.OrderBy(x => x.LogLevel)
                        : x => x.OrderByDescending(x => x.LogLevel);
                    break;
                case nameof(AppLog.EventName):
                    orderBy = filter.IsOrderAsc
                        ? x => x.OrderBy(x => x.EventName)
                        : x => x.OrderByDescending(x => x.EventName);
                    break;
                case nameof(AppLog.Source):
                    orderBy = filter.IsOrderAsc
                        ? x => x.OrderBy(x => x.Source)
                        : x => x.OrderByDescending(x => x.Source);
                    break;
                case nameof(AppLog.CreatedDateTime):
                    orderBy = filter.IsOrderAsc
                        ? x => x.OrderBy(x => x.CreatedDateTime)
                        : x => x.OrderByDescending(x => x.CreatedDateTime);
                    break;
                case null:
                    orderBy = null;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return orderBy;
        }
    }
}
