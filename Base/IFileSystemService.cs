using System;
using System.Collections.Generic;
using System.Diagnostics;
using FileArchiver.Helpers;

namespace FileArchiver.Base {
    public enum FileSystemEntryType {
        File,
        Directory
    }

    [DebuggerDisplay("FileSystemEntry(Type={Type},Name={Name},Path={Path})")]
    public struct FileSystemEntry : IEquatable<FileSystemEntry> {
        public readonly FileSystemEntryType Type;
        public readonly string Name;
        public readonly string Path;
        public readonly int Cardinality;

        public FileSystemEntry(FileSystemEntryType type, string name, string path, int cardinality = 0) {
            Type = type;
            Name = name;
            Path = path;
            Cardinality = cardinality;
        }

        #region Equals & GetHashCode

        public bool Equals(FileSystemEntry other) {
            return Type == other.Type && Cardinality == other.Cardinality
                                      && StringHelper.AreEqual(Name, other.Name)
                                      && StringHelper.AreEqual(Path, other.Path);
        }
        public override bool Equals(object obj) {
            return obj is FileSystemEntry other && Equals(other);
        }
        public override int GetHashCode() {
            return 0;
        }

        #endregion
    }

    public interface IFileSystemService {
        IEnumerable<FileSystemEntry> EnumFileSystemEntries(string path);
    }
}