using LteVideoPlayer.Api.Models.DataTypes;
using LteVideoPlayer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Entities
{
    public class ThumbnailError
    {
        public int Id { get; set; }
        public int TimesFailed { get; set; }
        public required DirectoryEnum DirectoryEnum { get; set; }
        public required string Error { get; set; }
        public required FileDataType File { get; set; }
        public DateTime LastError { get; set; } = DateTime.Now;
    }
}
