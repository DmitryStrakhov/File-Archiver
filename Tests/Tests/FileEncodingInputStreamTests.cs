using System;
using System.IO;
using FileArchiver.Core.FileCore;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture]
    public class FileEncodingInputStreamTests {
        [Test]
        public void CtorGuardCase1Test() {
            Assert.Throws<ArgumentNullException>(() => new FileEncodingInputStream(null, new TestIPlatformService()));
        }
        [Test]
        public void CtorGuardCase2Test() {
            Assert.Throws<ArgumentException>(() => new FileEncodingInputStream(string.Empty, new TestIPlatformService()));
        }
        [Test]
        public void CtorGuardCase3Test() {
            Assert.Throws<ArgumentNullException>(() => new FileEncodingInputStream("file", null));
        }
        [Test]
        public void ReadSymbolTest() {
            byte[] data = {0xFF, 0x33, 0x00, 0x12, 0x78};
            byte symbol;

            using(FileEncodingInputStream stream = CreateFileEncodingInputStream(data)) {
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
        [Test]
        public void ResetTest() {
            byte[] data = {0x12, 0x35, 0x01, 0x14};
            byte symbol;

            using(FileEncodingInputStream stream = CreateFileEncodingInputStream(data)) {
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
        private FileEncodingInputStream CreateFileEncodingInputStream(byte[] data) {
            TestIPlatformService platform = new TestIPlatformService {ReadFileFunc = x => new MemoryStream(data)};
            return new FileEncodingInputStream("file", platform);
        }
    }
}
