using System;
using System.IO;
using FileArchiver.FileCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class FileDecodingOutputStreamTests {
        [TestMethod]
        public void CtorGuardCase1Test() {
            AssertHelper.Throws<ArgumentNullException>(() => new FileDecodingOutputStream(null, new TestIPlatformService()));
        }
        [TestMethod]
        public void CtorGuardCase2Test() {
            AssertHelper.Throws<ArgumentException>(() => new FileDecodingOutputStream(string.Empty, new TestIPlatformService()));
        }
        [TestMethod]
        public void CtorGuardCase3Test() {
            AssertHelper.Throws<ArgumentNullException>(() => new FileDecodingOutputStream("file", null));
        }
        [TestMethod]
        public void WriteSymbolTest() {
            MemoryStream memoryStream = new MemoryStream();

            using(FileDecodingOutputStream stream = CreateFileDecodingOutputStream(memoryStream)) {
                stream.WriteSymbol(001);
                stream.WriteSymbol(005);
                stream.WriteSymbol(250);
                stream.WriteSymbol(255);
                stream.WriteSymbol(0);
                AssertHelper.AreEqual(new byte[] {1, 5, 250, 255, 0}, memoryStream.ToArray());
            }
        }
        private FileDecodingOutputStream CreateFileDecodingOutputStream(Stream stream) {
            TestIPlatformService platform = new TestIPlatformService {WriteFileFunc = x => stream};
            return new FileDecodingOutputStream("file", platform);
        }
    }
}
