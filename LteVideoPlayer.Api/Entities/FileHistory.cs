using LteVideoPlayer.Api.DataTypes;
using System.ComponentModel.DataAnnotations.Schema;

namespace LteVideoPlayer.Api.Entities
{
    public class FileHistory
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public float PercentWatched { get; set; }
        public FileEntity FileEntity { get; set; } = new FileEntity();
        public DateTime StartedDate { get; set; }

        #region Nav Props

        [ForeignKey(nameof(UserProfileId))]
        public UserProfile UserProfile { get; set; }

        #endregion
    }
}
