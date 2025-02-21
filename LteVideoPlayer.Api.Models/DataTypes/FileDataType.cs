using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.DataTypes
{
    [Owned]
    [Serializable]
    public class FileDataType : IEquatable<FileDataType>, IComparable<FileDataType>
    {
        public required string Path { get; set; }
        public required string File { get; set; }
        [NotMapped]
        public string FullPath { get => System.IO.Path.Combine(Path, File); }
        [NotMapped]
        public string FileWOExt { get => System.IO.Path.GetFileNameWithoutExtension(File); }
        [NotMapped]
        public string FileExt { get => System.IO.Path.GetExtension(File); }

        public string RootPathFile(string rootPath)
            => System.IO.Path.Combine(rootPath, FullPath);

        public string RootPathFileWOExt(string rootPath)
            => System.IO.Path.Combine(rootPath, Path, FileWOExt);

        public bool Equals(FileDataType other) =>
            this == other;

        public int CompareTo(FileDataType other)
        {
            var compare = Path.CompareTo(other.Path);
            if (compare == 0)
                compare = File.CompareTo(other.File);

            return compare;
        }

        public override bool Equals(object obj) =>
            obj is FileDataType other ? Equals(other) : false;

        public static bool operator !=(FileDataType lhs, FileDataType rhs) =>
            !(lhs == rhs);

        public static bool operator ==(FileDataType lhs, FileDataType rhs) =>
            lhs.Path == rhs.Path && lhs.File == rhs.File;

        public static bool operator <(FileDataType left, FileDataType right) =>
            left.CompareTo(right) < 0;

        public static bool operator <=(FileDataType left, FileDataType right) =>
            left.CompareTo(right) <= 0;

        public static bool operator >(FileDataType left, FileDataType right) =>
            left.CompareTo(right) > 0;

        public static bool operator >=(FileDataType left, FileDataType right) =>
            left.CompareTo(right) >= 0;

        public override int GetHashCode() => HashCode.Combine(FullPath);

        public override string ToString() => FullPath;
    }
}
