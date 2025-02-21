using LteVideoPlayer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Spa.ViewModels
{
    public class Paginate : PaginateDto
    {
    }

    public class PaginateGeneric<T> : PaginateDto<T> where T : class
    {
    }

    public class PaginateGenericFilter<T> : PaginateFilterDto<T> where T : class
    {
    }
}
