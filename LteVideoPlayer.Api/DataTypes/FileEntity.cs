using Microsoft.EntityFrameworkCore;

namespace LteVideoPlayer.Api.DataTypes
{
    [Owned]
    [Serializable]
    public class FileEntity : IEquatable<FileEntity>, IComparable<FileEntity>
    {
        public string FilePath { get; set; } = "";
        public string FileName { get; set; } = "";
        public string FilePathName
        {
            get => Path.Combine(FilePath, FileName);
        }

        public bool Equals(FileEntity other) =>
            this == other;

        public int CompareTo(FileEntity other)
        {
            var compare = FilePath.CompareTo(other.FilePath);
            if (compare == 0)
                compare = FileName.CompareTo(other.FileName);

            return compare;
        }

        public override bool Equals(object obj) =>
            obj is FileEntity other ? Equals(other) : false;

        public static bool operator !=(FileEntity lhs, FileEntity rhs) =>
            !(lhs == rhs);

        public static bool operator ==(FileEntity lhs, FileEntity rhs) =>
            lhs.FilePath == rhs.FilePath && lhs.FileName == rhs.FileName;

        public static bool operator <(FileEntity left, FileEntity right) =>
            left.CompareTo(right) < 0;

        public static bool operator <=(FileEntity left, FileEntity right) =>
            left.CompareTo(right) <= 0;

        public static bool operator >(FileEntity left, FileEntity right) =>
            left.CompareTo(right) > 0;

        public static bool operator >=(FileEntity left, FileEntity right) =>
            left.CompareTo(right) >= 0;
        public override int GetHashCode() => HashCode.Combine(FilePathName);
    }
}
