using System;
using System.IO;
using FileArchiver.DataStructures;
using FileArchiver.FileCore;
using FileArchiver.HuffmanCore;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture]
    public class FileEncodingOutputStreamTests {
        [Test]
        public void CtorGuardCase1Test() {
            Assert.Throws<ArgumentNullException>(() => new FileEncodingOutputStream(null, new TestIPlatformService()));
        }
        [Test]
        public void CtorGuardCase2Test() {
            Assert.Throws<ArgumentException>(() => new FileEncodingOutputStream(string.Empty, new TestIPlatformService()));
        }
        [Test]
        public void CtorGuardCase3Test() {
            Assert.Throws<ArgumentNullException>(() => new FileEncodingOutputStream("file", null));
        }
        [Test]
        public void WriteBitTest1() {
            MemoryStream memoryStream = new MemoryStream();

            using(FileEncodingOutputStream stream = CreateFileEncodingOutputStream(memoryStream)) {
                Bit[] bits = TestHelper.BitsFromString("1001110000110011");
                
                stream.BeginWrite();
                for(int n = 0; n < bits.Length; n++) {
                    stream.WriteBit(bits[n]);
                }
                stream.EndWrite();
            }
            Assert.AreEqual(new byte[] { 0x39, 0xCC}, memoryStream.ToArray());
        }
        [Test]
        public void WriteBitTest2() {
            MemoryStream memoryStream = new MemoryStream();

            using(FileEncodingOutputStream stream = CreateFileEncodingOutputStream(memoryStream)) {
                Bit[] bits = TestHelper.BitsFromString("01111001101");

                stream.BeginWrite();
                for(int n = 0; n < bits.Length; n++) {
                    stream.WriteBit(bits[n]);
                }
                stream.EndWrite();
            }
            Assert.AreEqual(new byte[] { 0x9E, 0x5}, memoryStream.ToArray());
        }

        [Test]
        public void RestorePositionGuardTest() {
            using(FileEncodingOutputStream stream = CreateFileEncodingOutputStream(new MemoryStream())) {
                Assert.Throws<ArgumentNullException>(() => stream.RestorePosition(null));
            }
        }
        [Test]
        public void SaveRestorePositionTest1() {
            MemoryStream memoryStream = new MemoryStream();

            using(FileEncodingOutputStream stream = CreateFileEncodingOutputStream(memoryStream)) {
                IStreamPosition position = stream.SavePosition();
                Assert.IsNotNull(position);
                stream.RestorePosition(position);
                
                WriteBitString(stream, "11111111");
                Assert.AreEqual(new byte[] {0xFF}, memoryStream.ToArray());
            }
        }
        [Test]
        public void SaveRestorePositionTest2() {
            MemoryStream memoryStream = new MemoryStream();

            using(FileEncodingOutputStream stream = CreateFileEncodingOutputStream(memoryStream)) {
                IStreamPosition p1 = stream.SavePosition();
                WriteBitString(stream, "11111111");
                IStreamPosition p2 = stream.SavePosition();
                WriteBitString(stream, "00000000");
                IStreamPosition p3 = stream.SavePosition();
                WriteBitString(stream, "11111111");
                IStreamPosition p4 = stream.SavePosition();

                stream.RestorePosition(p2);
                WriteBitString(stream, "10010110");
                stream.RestorePosition(p1);
                WriteBitString(stream, "10011001");
                stream.RestorePosition(p4);
                WriteBitString(stream, "00000001");
                stream.RestorePosition(p3);
                WriteBitString(stream, "00001111");
                Assert.AreEqual(new byte[] {0x99, 0x69, 0xF0, 0x80}, memoryStream.ToArray());
            }
        }

        private FileEncodingOutputStream CreateFileEncodingOutputStream(Stream stream) {
            TestIPlatformService platform = new TestIPlatformService {WriteFileFunc = x => stream};
            return new FileEncodingOutputStream("file", platform);
        }
        private void WriteBitString(FileEncodingOutputStream stream, string bitString) {
            Bit[] bits = TestHelper.BitsFromString(bitString);

            for(int n = 0; n < bits.Length; n++) {
                stream.WriteBit(bits[n]);
            }
        }
    }
}
