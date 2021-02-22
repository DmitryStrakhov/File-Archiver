using System.Diagnostics;

namespace FileArchiver.Core.HuffmanCore {
    [DebuggerDisplay("EncodingStatistics(InputSize={" + nameof(InputSize) + "}, , OutputSize={" + nameof(OutputSize) + "}")]
    public class EncodingStatistics {
        public EncodingStatistics(long inputSize, long outputSize) {
            InputSize = inputSize;
            OutputSize = outputSize;
        }
        public long InputSize { get; }
        public long OutputSize { get; }
    }
}