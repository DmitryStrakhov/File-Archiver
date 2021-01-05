using System;
using FileArchiver.Core.Format;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.Parsers {
    public interface IStreamParser {
        BootstrapSegment ParseWeightsTable(IDecodingInputStream stream);
        DirectorySegment ParseDirectory(IDecodingInputStream stream);
        FileSegment ParseFile(IDecodingInputStream stream, WeightsTable weightsTable);
    }
}