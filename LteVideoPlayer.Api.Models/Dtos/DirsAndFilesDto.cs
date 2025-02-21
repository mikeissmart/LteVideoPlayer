using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos
{
    public class DirsAndFilesDto : IRefactorType
    {
        public List<DirDto> Dirs { get; set; } = new List<DirDto>();
        public List<FileDto> Files { get; set; } = new List<FileDto>();
    }
}
