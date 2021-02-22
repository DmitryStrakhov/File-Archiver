using System;
using System.Threading;
using System.Threading.Tasks;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.Base {
    public interface IHuffmanEncodingService {
        Task<EncodingStatistics> EncodeAsync(string inputPath, string outputFile, CancellationToken cancellationToken, IProgress<CodingProgressInfo> progress);
    }
}