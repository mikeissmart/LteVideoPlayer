using LteVideoPlayer.Api.Models.DataTypes;
using LteVideoPlayer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Entities
{
    public class ConvertFile
    {
        public int Id { get; set; }
        public required DirectoryEnum DirectoryEnum { get; set; }
        public string? Output { get; set; }
        public bool Errored { get; set; }
        public required FileDataType OriginalFile { get; set; }
        public required FileDataType ConvertedFile { get; set; }
        public int AudioStreamIndex { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? StartedDate { get; set; }
        public DateTime? EndedDate { get; set; }
    }
}
