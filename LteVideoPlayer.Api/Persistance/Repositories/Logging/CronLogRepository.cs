using LteVideoPlayer.Api.Persistance;
using LteVideoPlayer.Api.Models;
using LteVideoPlayer.Api.Models.Dtos.Logging;
using LteVideoPlayer.Api.Models.Entities.Logging;
using Microsoft.EntityFrameworkCore;

namespace LteVideoPlayer.Api.Persistance.Repositories.Logging
{
    public interface ICronLogRepository : IRepository<CronLog>
    {
        Task<List<string>> AllNamesAsync();
        Task<Paginate<CronLog>> GetPaginatedLogsAsync(PaginateFilterDto<CronLogFilterDto> filter);
        Task<List<CronLog>> GetLogsBerforeDateAsync(DateTime date);
        Task<List<CronLog>> GetLastLogForCronJobsAsync(List<string> cronJobLogNames);
    }

    public class CronLogRepository : BaseRepository<CronLog>, ICronLogRepository
    {
        public CronLogRepository(LogDbContext dbContext) : base(dbContext)
        { }

        public async Task<List<string>> AllNamesAsync() =>
            await GenerateQuery()
                .Select(x => x.Name)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

        public async Task<Paginate<CronLog>> GetPaginatedLogsAsync(PaginateFilterDto<CronLogFilterDto> filter) =>
            await GetPaginateAsync(
                filter,
                predicate: x =>
                    (filter.Filter.IsCanceled == null || x.IsCanceled == filter.Filter.IsCanceled.Value) &&
                    (string.IsNullOrEmpty(filter.Filter.Name) || x.Name.Contains(filter.Filter.Name)) &&
                    (filter.Filter.MinDate == null || x.StartDateTime < filter.Filter.MinDate.Value) &&
                    (filter.Filter.MaxDate == null || x.StartDateTime >= filter.Filter.MaxDate.Value) &&
                    (filter.Filter.CronLogType == null || x.CronLogType == filter.Filter.CronLogType),
                orderBy: CalculateOrderBy(filter));

        public async Task<List<CronLog>> GetLogsBerforeDateAsync(DateTime date) =>
            await GetAsync(x => x.StartDateTime < date);

        public async Task<List<CronLog>> GetLastLogForCronJobsAsync(List<string> cronJobLogNames) =>
            await GenerateQuery(x => cronJobLogNames.Contains(x.Name))
                .GroupBy(x => x.Name, x => x)
                .Select(x => x.OrderByDescending(x => x.StartDateTime).First())
                .ToListAsync();

        private Func<IQueryable<CronLog>, IOrderedQueryable<CronLog>>? CalculateOrderBy(PaginateFilterDto filter)
        {
            Func<IQueryable<CronLog>, IOrderedQueryable<CronLog>>? orderBy = null;
            switch (filter.OrderBy)
            {
                case nameof(CronLog.Name):
                    orderBy = filter.IsOrderAsc
                        ? x => x.OrderBy(x => x.Name)
                        : x => x.OrderByDescending(x => x.Name);
                    break;
                case nameof(CronLog.IsCanceled):
                    orderBy = filter.IsOrderAsc
                        ? x => x.OrderBy(x => x.IsCanceled)
                        : x => x.OrderByDescending(x => x.IsCanceled);
                    break;
                case nameof(CronLog.StartDateTime):
                    orderBy = filter.IsOrderAsc
                        ? x => x.OrderBy(x => x.StartDateTime)
                        : x => x.OrderByDescending(x => x.StartDateTime);
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
