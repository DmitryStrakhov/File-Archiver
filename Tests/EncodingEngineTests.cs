#if DEBUG

using System;
using System.IO;
using FileArchiver.FileCore;
using FileArchiver.HuffmanCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class EncodingEngineTests {
        [TestMethod]
        public void Test1() {
            byte[] input = {0xEF, 0xBB, 0xBF, 0x3C, 0x57, 0x69, 0x6E, 0x64, 0x6F, 0x77, 0x3E};

            byte[] code = Encode(input, out HuffmanTreeBase tree);
            byte[] result = Decode(code, tree);
            CollectionAssert.AreEqual(input, result);
        }

        private byte[] Encode(byte[] data, out HuffmanTreeBase tree) {
            MemoryStream outputMemoryStream = new MemoryStream();
            FileEncodingInputStream inputStream = new FileEncodingInputStream(new MemoryStream(data));
            FileEncodingOutputStream outputStream = new FileEncodingOutputStream(outputMemoryStream);

            try {
                outputStream.BeginWrite();
                new HuffmanEncoder().Encode(inputStream, outputStream, out tree);
                outputStream.EndWrite();
                return outputMemoryStream.ToArray();
            }
            finally {
                inputStream.Dispose();
                outputStream.Dispose();
            }
        }
        private byte[] Decode(byte[] code, HuffmanTreeBase tree) {
            MemoryStream outputMemoryStream = new MemoryStream();
            FileDecodingInputStream inputStream = new FileDecodingInputStream(new MemoryStream(code));
            FileDecodingOutputStream outputStream = new FileDecodingOutputStream(outputMemoryStream);

            try {
                inputStream.BeginRead();
                new HuffmanDecoder().Decode(inputStream, tree.CastTo<HuffmanTree>() /*ToDo*/, outputStream);
                inputStream.EndRead();
                return outputMemoryStream.ToArray();
            }
            finally {
                inputStream.Dispose();
                outputStream.Dispose();
            }
        }
    }
}
#endif