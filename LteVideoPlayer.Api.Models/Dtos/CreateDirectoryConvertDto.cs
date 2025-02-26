using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos
{
    public class CreateDirectoryConvertDto : IRefactorType
    {
        public List<CreateFileConvertDto> Converts { get; set; } = new List<CreateFileConvertDto>();
    }
}
