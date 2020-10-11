using FileArchiver.Format;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Parsers {
    public interface IStreamParser {
        BootstrapSegment ParseWeightsTable(IDecodingInputStream stream);
        DirectorySegment ParseDirectory(IDecodingInputStream stream);
        FileSegment ParseFile(IDecodingInputStream stream, WeightsTable weightsTable);
    }
}