using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileArchiver.Core.Base {
    public interface IHuffmanEncodingService {
        Task<bool> EncodeAsync(string inputPath, string outputFile, CancellationToken cancellationToken, IProgress<CodingProgressInfo> progress);
    }
}