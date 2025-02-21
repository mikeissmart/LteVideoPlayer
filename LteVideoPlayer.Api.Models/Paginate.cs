using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models
{
    public class Paginate<T>
    {
        public int PageIndex { get; set; }
        public int PageItemCount { get; set; }
        public int TotalPages { get; set; }
        public int TotalItemCount { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
