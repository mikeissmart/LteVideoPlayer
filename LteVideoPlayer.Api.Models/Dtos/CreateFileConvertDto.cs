using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos
{
    public class CreateFileConvertDto :
        IRefactorType,
        IMapTo<ConvertFile>
    {
        public required DirectoryEnum DirectoryEnum { get; set; }
        public required FileDto OriginalFile { get; set; }
        public required FileDto ConvertedFile { get; set; }
        public int AudioStreamNumber { get; set; }
    }
}
