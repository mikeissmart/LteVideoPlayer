using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models
{
    // RefactorTypes are in ViewModels.GenericDtos
    public class PaginateDto
    {
        public int PageIndex { get; set; }
        public int PageItemCount { get; set; }
        public int TotalPages { get; set; }
        public int TotalItemCount { get; set; }
    }

    // Map is defined in Common.Mapper.MappingProfile
    public class PaginateDto<T> : PaginateDto
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }

    public static class PaginateDtoExtensions
    {
        public static PaginateDto<T> ToPaginate<T>(this IList<T> items, PaginateFilterDto paginateFilter) where T : class
        {
            var paginate = new PaginateDto<T>
            {
                PageIndex = paginateFilter.PageIndex,
                PageItemCount = paginateFilter.PageItemCount,
                TotalItemCount = items.Count()
            };

            if (paginateFilter.IsPaginated)
            {
                paginate.TotalPages = paginate.TotalItemCount / paginateFilter.PageItemCount + (
                    paginate.TotalItemCount % paginateFilter.PageItemCount > 0
                        ? 1
                        : 0);

                paginate.Items = items
                    .Skip(paginateFilter.PageIndex * paginateFilter.PageItemCount)
                    .Take(paginateFilter.PageItemCount);
            }
            else
            {
                paginate.TotalPages = 1;
                paginate.Items = items;
            }

            return paginate;
        }
    }
}
