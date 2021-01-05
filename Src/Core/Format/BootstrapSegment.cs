using System;
using System.Diagnostics;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.Format {
    [DebuggerDisplay("BootstrapSegment()")]
    public class BootstrapSegment {
        public BootstrapSegment(WeightsTable weightsTable) {
            Guard.IsNotNull(weightsTable, nameof(weightsTable));
            WeightsTable = weightsTable;
        }
        public WeightsTable WeightsTable { get; }
    }
}