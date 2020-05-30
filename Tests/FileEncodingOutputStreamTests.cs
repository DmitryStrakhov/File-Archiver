#if DEBUG

using System.IO;
using FileArchiver.DataStructures;
using FileArchiver.FileCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class FileEncodingOutputStreamTests {
        [TestMethod]
        public void WriteBitTest1() {
            MemoryStream memoryStream = new MemoryStream();

            using(FileEncodingOutputStream stream = new FileEncodingOutputStream(memoryStream)) {
                Bit[] bits = TestHelper.BitsFromString("1001110000110011");
                
                stream.BeginWrite();
                for(int n = 0; n < bits.Length; n++) {
                    stream.WriteBit(bits[n]);
                }
                stream.EndWrite();
            }
            AssertHelper.AreEqual(new byte[] { 0x10, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x39, 0xCC}, memoryStream.ToArray());
        }
        [TestMethod]
        public void WriteBitTest2() {
            MemoryStream memoryStream = new MemoryStream();

            using(FileEncodingOutputStream stream = new FileEncodingOutputStream(memoryStream)) {
                Bit[] bits = TestHelper.BitsFromString("01111001101");

                stream.BeginWrite();
                for(int n = 0; n < bits.Length; n++) {
                    stream.WriteBit(bits[n]);
                }
                stream.EndWrite();
            }
            AssertHelper.AreEqual(new byte[] { 0xB, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x9E, 0x5}, memoryStream.ToArray());
        }
    }
}
#endif