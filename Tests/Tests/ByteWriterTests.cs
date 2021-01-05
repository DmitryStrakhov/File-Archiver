using System;
using FileArchiver.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class ByteWriterTests {
        ByteWriter writer;
        
        [TestInitialize]
        public void SetUp() {
            this.writer = new ByteWriter();
        }

        [TestMethod]
        public void DefaultsTest() {
            Assert.AreEqual(0, writer.Value);
            Assert.IsFalse(writer.IsReady);
            Assert.IsTrue(writer.IsEmpty);
        }
        [TestMethod]
        public void AddBitGuardTest() {
            for(int n = 0; n < 8; n++) {
                writer.AddBit(Bit.Zero);
            }
            AssertHelper.Throws<InvalidOperationException>(() => writer.AddBit(Bit.Zero));
        }
        [TestMethod]
        public void AddBitTest1() {
            writer.AddBit(Bit.One);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.One);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.One);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.One);
            writer.AddBit(Bit.Zero);
            Assert.AreEqual(85, writer.Value);
        }
        [TestMethod]
        public void AddBitTest2() {
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.One);
            writer.AddBit(Bit.One);
            writer.AddBit(Bit.One);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.One);
            writer.AddBit(Bit.Zero);
            Assert.AreEqual(92, writer.Value);
        }
        [TestMethod]
        public void AddBitTest3() {
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.One);
            Assert.AreEqual(128, writer.Value);
        }
        [TestMethod]
        public void IsReadyTest() {
            Assert.IsFalse(writer.IsReady);
            writer.AddBit(Bit.Zero);
            Assert.IsFalse(writer.IsReady);
            writer.AddBit(Bit.Zero);
            Assert.IsFalse(writer.IsReady);
            writer.AddBit(Bit.Zero);
            Assert.IsFalse(writer.IsReady);
            writer.AddBit(Bit.Zero);
            Assert.IsFalse(writer.IsReady);
            writer.AddBit(Bit.Zero);
            Assert.IsFalse(writer.IsReady);
            writer.AddBit(Bit.Zero);
            Assert.IsFalse(writer.IsReady);
            writer.AddBit(Bit.Zero);
            Assert.IsFalse(writer.IsReady);
            writer.AddBit(Bit.Zero);
            Assert.IsTrue(writer.IsReady);
        }
        [TestMethod]
        public void ResetTest1() {
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            writer.Reset();
            Assert.AreEqual(0, writer.Value);
        }
        [TestMethod]
        public void ResetTest2() {
            for(int n = 0; n < 8; n++) {
                writer.AddBit(Bit.Zero);
            }
            writer.Reset();
            Assert.IsFalse(writer.IsReady);
            writer.AddBit(Bit.One);
            writer.AddBit(Bit.One);
            writer.AddBit(Bit.One);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            writer.AddBit(Bit.Zero);
            Assert.AreEqual(7, writer.Value);
        }
        [TestMethod]
        public void IsEmptyTest() {
            Assert.IsTrue(writer.IsEmpty);

            for(int n = 0; n < 8; n++) {
                writer.AddBit(Bit.Zero);
                Assert.IsFalse(writer.IsEmpty);
            }
            writer.Reset();
            Assert.IsTrue(writer.IsEmpty);
        }
    }
}
