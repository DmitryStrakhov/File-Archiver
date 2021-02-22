using FileArchiver.Core.FormatProviders;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture]
    public class FileSizeFormatProviderTests {
        [Test]
        public void SimpleTest() {
            string @string = string.Format(FileSizeFormatProvider.Instance, "Text: {0:FS}", 128L);
            Assert.AreEqual("Text: 128 Bytes", @string);
        }
        [Test]
        public void KilobytesTest() {
            string @string = string.Format(FileSizeFormatProvider.Instance, "Text: {0:FS}", 13552L);
            Assert.AreEqual("Text: 13.55 Kb", @string);
        }
        [Test]
        public void MegabytesTest() {
            string @string = string.Format(FileSizeFormatProvider.Instance, "Text: {0:FS}", 26855366L);
            Assert.AreEqual("Text: 26.86 Mb", @string);
        }
        [Test]
        public void GigabytesTest() {
            string @string = string.Format(FileSizeFormatProvider.Instance, "Text: {0:FS}", 156223559865L);
            Assert.AreEqual("Text: 156.22 Gb", @string);
        }
        [Test]
        public void CompatibilityTest() {
            string @string = string.Format(FileSizeFormatProvider.Instance, "{0:d}-Text: {1:FS}-{2:d}", 1, 58L, 2);
            Assert.AreEqual("1-Text: 58 Bytes-2", @string);
        }
    }
}