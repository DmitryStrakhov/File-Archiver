#if DEBUG

using System.IO;
using FileArchiver.FileCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class FileEncodingInputStreamTests {
        [TestMethod]
        public void ReadSymbolTest() {
            byte[] data = {0xFF, 0x33, 0x00, 0x12, 0x78};
            byte symbol;

            using(FileEncodingInputStream stream = new FileEncodingInputStream(new MemoryStream(data))) {
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0xFF, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x33, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x00, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x12, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x78, symbol);
                Assert.IsFalse(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x00, symbol);
                Assert.IsFalse(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x00, symbol);
            }
        }
        [TestMethod]
        public void ResetTest() {
            byte[] data = {0x12, 0x35, 0x01, 0x14};
            byte symbol;

            using(FileEncodingInputStream stream = new FileEncodingInputStream(new MemoryStream(data))) {
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x12, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x35, symbol);
                stream.Reset();

                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x12, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x35, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x01, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x14, symbol);
                Assert.IsFalse(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x00, symbol);
            }
        }
    }
}
#endif