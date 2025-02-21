using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos
{
    public class CreateManyConvertDto : IRefactorType
    {
        public List<CreateConvertDto> Converts { get; set; } = new List<CreateConvertDto>();
    }
}
