using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos.Remote
{
    public class RemoteMoveSeekDto : RemoteDto, IRefactorType
    {
        public float Seek { get; set; }
    }
}
