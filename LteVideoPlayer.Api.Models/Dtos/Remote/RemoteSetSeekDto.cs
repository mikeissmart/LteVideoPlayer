using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos.Remote
{
    public class RemoteSetSeekDto : RemoteDto, IRefactorType
    {
        public float SeekPercent { get; set; }
    }
}
