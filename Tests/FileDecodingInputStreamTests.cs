#if DEBUG

using System.Collections.Generic;
using System.IO;
using FileArchiver.DataStructures;
using FileArchiver.FileCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class FileDecodingInputStreamTests {
        [TestMethod]
        public void ReadBitTest1() {
            byte[] data = {0x10, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x39, 0xCC};

            using(FileDecodingInputStream stream = new FileDecodingInputStream(new MemoryStream(data))) {
                List<Bit> bitList = new List<Bit>(16);

                stream.BeginRead();
                while(!stream.IsEmpty) {
                    Assert.IsTrue(stream.ReadBit(out Bit bit));
                    bitList.Add(bit);
                }
                Assert.IsFalse(stream.ReadBit(out Bit _));
                stream.EndRead();
                Assert.AreEqual("1001110000110011", TestHelper.StringFromBits(bitList.ToArray()));
            }
        }
        [TestMethod]
        public void ReadBitTest2() {
            byte[] data = {0xB, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x9E, 0x5};

            using(FileDecodingInputStream stream = new FileDecodingInputStream(new MemoryStream(data))) {
                List<Bit> bitList = new List<Bit>(16);

                stream.BeginRead();
                while(!stream.IsEmpty) {
                    Assert.IsTrue(stream.ReadBit(out Bit bit));
                    bitList.Add(bit);
                }
                Assert.IsFalse(stream.ReadBit(out Bit _));
                stream.EndRead();
                Assert.AreEqual("01111001101", TestHelper.StringFromBits(bitList.ToArray()));
            }
        }
    }
}
#endif