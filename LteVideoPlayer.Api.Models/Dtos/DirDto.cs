using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos
{
    public class DirDto : IRefactorType
    {
        public string Path { get; set; } = "";
        public string Name { get; set; } = "";
        public string FullPath { get => System.IO.Path.Combine(Path, Name); }
    }
}
