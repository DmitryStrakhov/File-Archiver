#if DEBUG

using System;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Tests {
    class EmptyEncodingToken : EncodingToken {
        private EmptyEncodingToken()
            : base(new WeightsTable(), EmptyHuffmanTree.Instance, new CodingTable()) {
        }
        public static readonly EmptyEncodingToken Instance = new EmptyEncodingToken();
    }
}
#endif