using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileArchiver.Core.Base {
    public interface IHuffmanDecodingService {
        Task DecodeAsync(string inputFile, string outputFolder, CancellationToken cancellationToken, IProgress<CodingProgressInfo> progress);
    }
}