using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos.Remote
{
    public class RemoteSetVolumeDto : RemoteDto, IRefactorType
    {
        public float Volume { get; set; }
    }
}
