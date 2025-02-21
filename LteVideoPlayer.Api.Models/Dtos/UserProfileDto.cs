using LteVideoPlayer.Api.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos
{
    public class UserProfileDto :
        IRefactorType,
        IMapFrom<UserProfile>,
        IMapTo<UserProfile>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
