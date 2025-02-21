using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Entities
{
    public class UserProfile
    {
        public int Id { get; set; }
        [StringLength(50)]
        public required string Name { get; set; }
    }
}
