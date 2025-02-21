using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models
{
    public class PaginateFilterDto
    {
        public int PageIndex { get; set; }
        [Range(1, int.MaxValue)]
        public int PageItemCount { get; set; }
        public string OrderBy { get; set; } = "";
        public bool IsOrderAsc { get; set; }
        public bool IsPaginated { get; set; } = true;
    }

    public class PaginateFilterDto<T> : PaginateFilterDto where T : class
    {
        public T Filter { get; set; }
    }
}
