using LteVideoPlayer.Api.DataTypes;
using System.ComponentModel.DataAnnotations.Schema;

namespace LteVideoPlayer.Api.Entities
{
    public class ConvertFile
    {
        public int Id { get; set; }
        public string? Output { get; set; }
        public bool Errored { get; set; }
        public FileEntity OriginalFile { get; set; } = new FileEntity();
        public FileEntity ConvertedFile { get; set; } = new FileEntity();
        public int AudioStream { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? StartedDate { get; set; }
        public DateTime? EndedDate { get; set; }
    }
}
