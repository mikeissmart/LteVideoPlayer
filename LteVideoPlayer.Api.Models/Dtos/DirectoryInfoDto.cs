using LteVideoPlayer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos
{
    public class DirectoryInfoDto : IRefactorType
    {
        public string FriendlyName { get; set; } = "";
        public bool CanPlayVideo { get; set; }
        public bool CanConvertVideo { get; set; }
        public bool CanThumbnailVideo { get; set; }
        public bool AdminViewOnly { get; set; }
        public DirectoryEnum DirEnum { get; set; }
    }
}
