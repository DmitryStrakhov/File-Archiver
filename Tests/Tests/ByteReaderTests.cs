using System;
using FileArchiver.DataStructures;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture]
    public class ByteReaderTests {
        ByteReader reader;

        [SetUp]
        public void SetUp() {
            this.reader = new ByteReader();
        }
        [Test]
        public void DefaultsTest() {
            Assert.IsFalse(reader.IsReady);
        }
        [Test]
        public void ReadBitGuardCase1Test() {
            Assert.Throws<InvalidOperationException>(() => reader.ReadBit());
        }
        [Test]
        public void ReadBitGuardCase2Test() {
            reader.SetValue(1);

            for(int n = 0; n < 8; n++) {
                reader.ReadBit();
            }
            Assert.Throws<InvalidOperationException>(() => reader.ReadBit());
        }
        [Test]
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
        [Test]
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
        [Test]
        public void IsReadyTest() {
            reader.SetValue(0);
            for(int n = 0; n < 8; n++) {
                Assert.IsTrue(reader.IsReady);
                reader.ReadBit();
            }
            Assert.IsFalse(reader.IsReady);
        }
        [Test]
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
