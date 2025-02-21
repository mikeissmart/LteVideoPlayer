using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos
{
    public class MetadataDto : IRefactorType
    {
        public string Output { get; set; } = "";
        public string Error { get; set; } = "";
    }
}
