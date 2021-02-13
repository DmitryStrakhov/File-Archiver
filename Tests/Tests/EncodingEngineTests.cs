using System;
using System.IO;
using FileArchiver.Core.FileCore;
using FileArchiver.Core.HuffmanCore;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture]
    public class EncodingEngineTests {
        [Test]
        public void Test1() {
            byte[] input = {0xEF, 0xBB, 0xBF, 0x3C, 0x57, 0x69, 0x6E, 0x64, 0x6F, 0x77, 0x3E};

            byte[] code = Encode(input, out HuffmanTreeBase tree, out long streamSize);
            byte[] result = Decode(code, tree, streamSize);
            CollectionAssert.AreEqual(input, result);
        }

        private byte[] Encode(byte[] data, out HuffmanTreeBase tree, out long streamSize) {
            MemoryStream outputMemoryStream = new MemoryStream();
            FileEncodingInputStream inputStream = CreateFileEncodingInputStream(data);
            FileEncodingOutputStream outputStream = CreateFileEncodingOutputStream(outputMemoryStream);

            HuffmanEncoder encoder = new HuffmanEncoder();
            try {
                outputStream.BeginWrite();
                EncodingToken token = encoder.CreateEncodingToken(inputStream);
                streamSize = encoder.Encode(inputStream, outputStream, token, null);
                outputStream.EndWrite();
                tree = token.HuffmanTree;
                return outputMemoryStream.ToArray();
            }
            finally {
                inputStream.Dispose();
                outputStream.Dispose();
            }
        }
        private byte[] Decode(byte[] code, HuffmanTreeBase tree, long streamSize) {
            MemoryStream outputMemoryStream = new MemoryStream();
            FileDecodingInputStream inputStream = CreateFileDecodingInputStream(code);
            FileDecodingOutputStream outputStream = CreateFileDecodingOutputStream(outputMemoryStream);

            try {
                new HuffmanDecoder().Decode(inputStream, tree, outputStream, streamSize, null);
                return outputMemoryStream.ToArray();
            }
            finally {
                inputStream.Dispose();
                outputStream.Dispose();
            }
        }

        private FileEncodingInputStream CreateFileEncodingInputStream(byte[] data) {
            TestIPlatformService platform = new TestIPlatformService {ReadFileFunc = x => new MemoryStream(data)};
            return new FileEncodingInputStream("file", platform);
        }
        private FileDecodingInputStream CreateFileDecodingInputStream(byte[] data) {
            TestIPlatformService platform = new TestIPlatformService {ReadFileFunc = x => new MemoryStream(data)};
            return new FileDecodingInputStream("file", platform);
        }
        private FileEncodingOutputStream CreateFileEncodingOutputStream(Stream stream) {
            TestIPlatformService platform = new TestIPlatformService {WriteFileFunc = x => stream};
            return new FileEncodingOutputStream("file", platform);
        }
        private FileDecodingOutputStream CreateFileDecodingOutputStream(Stream stream) {
            TestIPlatformService platform = new TestIPlatformService {WriteFileFunc = x => stream};
            return new FileDecodingOutputStream("file", platform);
        }
    }
}
