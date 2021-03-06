﻿using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Core.DataStructures;
using FileArchiver.Core.FileCore;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture]
    public class FileDecodingInputStreamTests {
        [Test]
        public void CtorGuardCase1Test() {
            Assert.Throws<ArgumentNullException>(() => new FileDecodingInputStream(null, new TestIPlatformService()));
        }
        [Test]
        public void CtorGuardCase2Test() {
            Assert.Throws<ArgumentException>(() => new FileDecodingInputStream(string.Empty, new TestIPlatformService()));
        }
        [Test]
        public void CtorGuardCase3Test() {
            Assert.Throws<ArgumentNullException>(() => new FileDecodingInputStream("file", null));
        }
        [Test]
        public void DefaultsTest() {
            using(FileDecodingInputStream stream = CreateFileDecodingInputStream(new byte[]{1})) {
                Assert.IsFalse(stream.IsEmpty);
                Assert.AreEqual(1, stream.SizeInBytes);
                Assert.AreEqual(0, stream.Position);
            }
        }
        [Test]
        public void EmptyStreamTest() {
            using(FileDecodingInputStream stream = CreateFileDecodingInputStream(new byte[0])) {
                Assert.IsTrue(stream.IsEmpty);
                Assert.AreEqual(0, stream.SizeInBytes);
                Assert.AreEqual(0, stream.Position);
            }
        }
        [Test]
        public void PositionPropertyTest() {
            byte[] data = {0x1, 0x2, 0x3};

            using(FileDecodingInputStream stream = CreateFileDecodingInputStream(data)) {
                Assert.AreEqual(0, stream.Position);

                stream.ReadBit(out Bit _);
                stream.ReadBit(out Bit _);
                Assert.AreEqual(2, stream.Position);
                stream.ReadBit(out Bit _);
                stream.ReadBit(out Bit _);
                stream.ReadBit(out Bit _);
                stream.ReadBit(out Bit _);
                stream.ReadBit(out Bit _);
                stream.ReadBit(out Bit _);
                Assert.AreEqual(8, stream.Position);
                stream.ReadBit(out Bit _);
                stream.ReadBit(out Bit _);
                Assert.AreEqual(10, stream.Position);
                stream.ReadBit(out Bit _);
                stream.ReadBit(out Bit _);
                stream.ReadBit(out Bit _);
                Assert.AreEqual(13, stream.Position);
            }
        }
        [Test]
        public void SizePropertyTest() {
            byte[] data = { 0x1, 0x2, 0x3 };

            using(FileDecodingInputStream stream = CreateFileDecodingInputStream(data)) {
                Assert.AreEqual(3, stream.SizeInBytes);
            }
        }
        [Test]
        public void ReadBitTest1() {
            byte[] data = {0x39, 0xCC};

            using(FileDecodingInputStream stream = CreateFileDecodingInputStream(data)) {
                List<Bit> bitList = new List<Bit>(16);

                while(!stream.IsEmpty) {
                    Assert.IsTrue(stream.ReadBit(out Bit bit));
                    bitList.Add(bit);
                }
                Assert.IsFalse(stream.ReadBit(out Bit _));
                Assert.AreEqual("1001110000110011", TestHelper.StringFromBits(bitList.ToArray()));
            }
        }
        [Test]
        public void ReadBitTest2() {
            byte[] data = {0x9E, 0x5};

            using(FileDecodingInputStream stream = CreateFileDecodingInputStream(data)) {
                List<Bit> bitList = new List<Bit>(16);

                while(!stream.IsEmpty) {
                    Assert.IsTrue(stream.ReadBit(out Bit bit));
                    bitList.Add(bit);
                }
                Assert.IsFalse(stream.ReadBit(out Bit _));
                Assert.AreEqual("0111100110100000", TestHelper.StringFromBits(bitList.ToArray()));
            }
        }
        private FileDecodingInputStream CreateFileDecodingInputStream(byte[] data) {
            TestIPlatformService platform = new TestIPlatformService {ReadFileFunc = x => new MemoryStream(data)};
            return new FileDecodingInputStream("file", platform);
        }
    }
}
