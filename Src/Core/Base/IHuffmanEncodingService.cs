using System;
using System.Threading.Tasks;

namespace FileArchiver.Core.Base {
    public interface IHuffmanEncodingService {
        Task<bool> EncodeAsync(string inputPath, string outputFile, IProgress<CodingProgressInfo> progress);
    }
}