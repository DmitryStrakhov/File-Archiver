#if DEBUG

using System.IO;
using FileArchiver.FileCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class FileDecodingOutputStreamTests {
        [TestMethod]
        public void WriteSymbolTest() {
            MemoryStream memoryStream = new MemoryStream();

            using(FileDecodingOutputStream stream = new FileDecodingOutputStream(memoryStream)) {
                stream.WriteSymbol(001);
                stream.WriteSymbol(005);
                stream.WriteSymbol(250);
                stream.WriteSymbol(255);
                stream.WriteSymbol(0);
                AssertHelper.AreEqual(new byte[] {1, 5, 250, 255, 0}, memoryStream.ToArray());
            }
        }
    }
}
#endif