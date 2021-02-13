using System;
using System.Diagnostics;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.Format {
    [DebuggerDisplay("FileSegment(Name: {" + nameof(Name) + "})")]
    public class FileSegment {
        public FileSegment(string name, string path) {
            Guard.IsNotNullOrEmpty(name, nameof(name));
            Guard.IsNotNullOrEmpty(path, nameof(path));

            Name = name;
            Path = path;
        }
        public FileSegment(string name, IFileDecoder fileDecoder) {
            Guard.IsNotNullOrEmpty(name, nameof(name));
            Guard.IsNotNull(fileDecoder, nameof(fileDecoder));

            Name = name;
            FileDecoder = fileDecoder;
        }

        public string Name { get; }
        public string Path { get; }
        public IFileDecoder FileDecoder { get; }
    }

    public interface IFileDecoder {
        void Decode(IDecodingOutputStream outputStream, IProgressHandler progress);
    }
}