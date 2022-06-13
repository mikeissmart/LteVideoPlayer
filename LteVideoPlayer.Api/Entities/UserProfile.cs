using System.ComponentModel.DataAnnotations;

namespace LteVideoPlayer.Api.Entities
{
    public class UserProfile
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }

        #region Nav Props

        public List<FileHistory> WatchVideos { get; set; }

        #endregion
    }
}
