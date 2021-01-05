using System;
using System.IO;
using FileArchiver.Core.FileCore;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture]
    public class FileDecodingOutputStreamTests {
        [Test]
        public void CtorGuardCase1Test() {
            Assert.Throws<ArgumentNullException>(() => new FileDecodingOutputStream(null, new TestIPlatformService()));
        }
        [Test]
        public void CtorGuardCase2Test() {
            Assert.Throws<ArgumentException>(() => new FileDecodingOutputStream(string.Empty, new TestIPlatformService()));
        }
        [Test]
        public void CtorGuardCase3Test() {
            Assert.Throws<ArgumentNullException>(() => new FileDecodingOutputStream("file", null));
        }
        [Test]
        public void WriteSymbolTest() {
            MemoryStream memoryStream = new MemoryStream();

            using(FileDecodingOutputStream stream = CreateFileDecodingOutputStream(memoryStream)) {
                stream.WriteSymbol(001);
                stream.WriteSymbol(005);
                stream.WriteSymbol(250);
                stream.WriteSymbol(255);
                stream.WriteSymbol(0);
                Assert.AreEqual(new byte[] {1, 5, 250, 255, 0}, memoryStream.ToArray());
            }
        }
        private FileDecodingOutputStream CreateFileDecodingOutputStream(Stream stream) {
            TestIPlatformService platform = new TestIPlatformService {WriteFileFunc = x => stream};
            return new FileDecodingOutputStream("file", platform);
        }
    }
}
