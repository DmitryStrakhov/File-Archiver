#if DEBUG

using System;
using FileArchiver.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class ByteReaderTests {
        ByteReader reader;

        [TestInitialize]
        public void SetUp() {
            this.reader = new ByteReader();
        }
        [TestMethod]
        public void DefaultsTest() {
            Assert.IsFalse(reader.IsReady);
        }
        [TestMethod]
        public void ReadBitGuardCase1Test() {
            AssertHelper.Throws<InvalidOperationException>(() => reader.ReadBit());
        }
        [TestMethod]
        public void ReadBitGuardCase2Test() {
            reader.SetValue(1);

            for(int n = 0; n < 8; n++) {
                reader.ReadBit();
            }
            AssertHelper.Throws<InvalidOperationException>(() => reader.ReadBit());
        }
        [TestMethod]
        public void ReadBitTest1() {
            reader.SetValue(189);
            Assert.AreEqual(Bit.One, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
            Assert.AreEqual(Bit.One, reader.ReadBit());
            Assert.AreEqual(Bit.One, reader.ReadBit());
            Assert.AreEqual(Bit.One, reader.ReadBit());
            Assert.AreEqual(Bit.One, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
            Assert.AreEqual(Bit.One, reader.ReadBit());
        }
        [TestMethod]
        public void ReadBitTest2() {
            reader.SetValue(23);
            Assert.AreEqual(Bit.One, reader.ReadBit());
            Assert.AreEqual(Bit.One, reader.ReadBit());
            Assert.AreEqual(Bit.One, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
            Assert.AreEqual(Bit.One, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
        }
        [TestMethod]
        public void IsReadyTest() {
            reader.SetValue(0);
            for(int n = 0; n < 8; n++) {
                Assert.IsTrue(reader.IsReady);
                reader.ReadBit();
            }
            Assert.IsFalse(reader.IsReady);
        }
        [TestMethod]
        public void ResetTest() {
            reader.SetValue(1);

            for(int n = 0; n < 8; n++) {
                reader.ReadBit();
            }
            reader.SetValue(1);
            Assert.IsTrue(reader.IsReady);
            Assert.AreEqual(Bit.One, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
            Assert.AreEqual(Bit.Zero, reader.ReadBit());
        }
    }
}
#endif