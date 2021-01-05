using System.Diagnostics;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Format {
    [DebuggerDisplay("BootstrapSegment()")]
    public class BootstrapSegment {
        public BootstrapSegment(WeightsTable weightsTable) {
            Guard.IsNotNull(weightsTable, nameof(weightsTable));
            WeightsTable = weightsTable;
        }
        public WeightsTable WeightsTable { get; }
    }
}