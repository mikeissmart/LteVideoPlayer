using LteVideoPlayer.Api.Models.DataTypes;
using LteVideoPlayer.Api.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos
{
    public class FileDataTypeDto :
        IRefactorType,
        IMapFrom<FileDataType>,
        IMapTo<FileDataType>
    {
        public required string Path { get; set; }
        public required string File { get; set; }
        public string FullPath { get => System.IO.Path.Combine(Path, File); }
        public string FileWOExt { get => System.IO.Path.GetFileNameWithoutExtension(File); }
        public string FileExt { get => System.IO.Path.GetExtension(File); }

        public override string ToString() => FullPath;
    }
}
