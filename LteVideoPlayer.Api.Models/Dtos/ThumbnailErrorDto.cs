using LteVideoPlayer.Api.Models.DataTypes;
using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos
{
    public class ThumbnailErrorDto :
        IRefactorType,
        IMapTo<ThumbnailError>,
        IMapFrom<ThumbnailError>
    {
        public int Id { get; set; }
        public int TimesFailed { get; set; }
        public DirectoryEnum DirectoryEnum { get; set; }
        public string Error { get; set; } = "";
        public FileDataTypeDto File { get; set; }
        public DateTime LastError { get; set; } = DateTime.Now;
    }
}
