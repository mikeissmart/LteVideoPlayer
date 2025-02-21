using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos
{
    public class ConvertManyFileDto : IRefactorType
    {
        public List<ConvertFileDto> Converts { get; set; } = new List<ConvertFileDto>();
    }
}
