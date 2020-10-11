﻿#if DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.DataStructures;
using FileArchiver.FileCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class FileDecodingInputStreamTests {
        [TestMethod]
        public void CtorGuardCase1Test() {
            AssertHelper.Throws<ArgumentNullException>(() => new FileDecodingInputStream(null, new TestIPlatformService()));
        }
        [TestMethod]
        public void CtorGuardCase2Test() {
            AssertHelper.Throws<ArgumentException>(() => new FileDecodingInputStream(string.Empty, new TestIPlatformService()));
        }
        [TestMethod]
        public void CtorGuardCase3Test() {
            AssertHelper.Throws<ArgumentNullException>(() => new FileDecodingInputStream("file", null));
        }
        [TestMethod]
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
        [TestMethod]
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
#endif