using LteVideoPlayer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos.Remote
{
    public class RemoteVideoInfoDto : RemoteDto, IRefactorType
    {
        public DirectoryEnum DirectoryEnum { get; set; }
        public string FriendlyName { get; set; } = "";
        public string Path { get; set; }
        public FileDto File { get; set; }
        public float CurrentTimeSeconds { get; set; }
        public float MaxTimeSeconds { get; set; }
        public int Volume { get; set; }
        public bool IsPlaying { get; set; }
    }
}
