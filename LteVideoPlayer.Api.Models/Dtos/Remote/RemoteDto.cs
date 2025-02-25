using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos.Remote
{
    public class RemoteDto : IRefactorType
    {
        public required string Profile { get; set; }
        public int FromChannel { get; set; }
        public int? ToChannel { get; set; }
    }
}
