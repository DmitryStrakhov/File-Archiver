using System;
using System.Collections;
using System.Linq;
using FileArchiver.DataStructures;
using FileArchiver.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class BitSequenceTests {
        BitSequence sequence;

        [TestInitialize]
        public void SetUp() {
            this.sequence = new BitSequence();
        }
        [TestCleanup]
        public void TearDown() {
            this.sequence = null;
        }
        [TestMethod]
        public void DefaultsTest() {
            Assert.AreEqual(0, sequence.Size);
            AssertHelper.CollectionIsEmpty(sequence);
        }
        [TestMethod]
        public void IndexerGetGuardTest1() {
            AssertHelper.Throws<ArgumentOutOfRangeException>((() => {
                Bit bit = sequence[-1];
            }));
        }
        [TestMethod]
        public void IndexerGetGuardTest2() {
            AssertHelper.Throws<ArgumentOutOfRangeException>((() => {
                Bit bit = sequence[0];
            }));
        }
        [TestMethod]
        public void IndexerGetGuardTest3() {
            sequence[0] = Bit.One;
            sequence[1] = Bit.Zero;
            sequence[2] = Bit.One;
            sequence[3] = Bit.Zero;
            AssertHelper.Throws<ArgumentOutOfRangeException>((() => {
                Bit bit = sequence[4];
            }));
        }
        [TestMethod]
        public void IndexerSetGuardTest() {
            AssertHelper.Throws<ArgumentOutOfRangeException>((() => sequence[-1] = Bit.One));
        }
        [TestMethod]
        public void SequenceSizeTest() {
            sequence[0] = Bit.Zero;
            Assert.AreEqual(1, sequence.Size);
            sequence[0] = Bit.One;
            Assert.AreEqual(1, sequence.Size);
            sequence[1] = Bit.One;
            Assert.AreEqual(2, sequence.Size);
            sequence[11] = Bit.One;
            Assert.AreEqual(12, sequence.Size);
            sequence[111] = Bit.One;
            Assert.AreEqual(112, sequence.Size);
        }
        [TestMethod]
        public void IndexerTest1() {
            sequence[0] = Bit.Zero;
            AssertSequence(0);
            sequence[0] = Bit.One;
            AssertSequence(1);
            sequence[1] = Bit.One;
            sequence[2] = Bit.One;
            AssertSequence(1, 1, 1);
        }
        [TestMethod]
        public void IndexerTest2() {
            sequence[0] = Bit.Zero;
            sequence[3] = Bit.One;
            sequence[5] = Bit.One;
            sequence[9] = Bit.One;
            AssertSequence(0, 0, 0, 1, 0, 1, 0, 0, 0, 1);
        }
        [TestMethod]
        public void IndexerTest3() {
            for(int n = 0; n < 100; n++) {
                sequence[n] = MathHelper.IsEven(n) ? Bit.Zero : Bit.One;
            }
            for(int n = 0; n < 100; n++) {
                Bit expectedBit = MathHelper.IsEven(n) ? Bit.Zero : Bit.One;
                Assert.AreEqual(expectedBit, sequence[n]);
            }
        }
        [TestMethod]
        public void CloneTest1() {
            BitSequence clonedSequence = sequence.Clone();
            Assert.AreEqual(0, clonedSequence.Size);
            AssertHelper.CollectionIsEmpty(clonedSequence);
        }
        [TestMethod]
        public void CloneTest2() {
            sequence[0] = Bit.One;
            sequence[3] = Bit.One;
            sequence[5] = Bit.Zero;
            sequence[11] = Bit.One;
            BitSequence clonedSequence = sequence.Clone();
            Assert.AreEqual(12, clonedSequence.Size);
            AssertSequence(clonedSequence, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1);
        }
        [TestMethod]
        public void ReduceGuardTest() {
            AssertHelper.Throws<ArgumentOutOfRangeException>(() => sequence.Reduce(-1));
        }
        [TestMethod]
        public void ReduceTest1() {
            sequence[0] = Bit.One;
            sequence[1] = Bit.Zero;
            sequence[2] = Bit.One;
            sequence[3] = Bit.Zero;

            sequence.Reduce(3);
            AssertSequence(1, 0, 1, 0);
            Assert.AreEqual(4, sequence.Size);
        }
        [TestMethod]
        public void ReduceTest2() {
            sequence[0] = Bit.One;
            sequence[1] = Bit.Zero;
            sequence[2] = Bit.One;
            sequence[3] = Bit.Zero;

            sequence.Reduce(10);
            AssertSequence(1, 0, 1, 0);
            Assert.AreEqual(4, sequence.Size);
        }
        [TestMethod]
        public void ReduceTest3() {
            sequence[0] = Bit.One;
            sequence[1] = Bit.Zero;
            sequence[2] = Bit.One;
            sequence[3] = Bit.Zero;

            sequence.Reduce(1);
            AssertSequence(1, 0);
            Assert.AreEqual(2, sequence.Size);

            sequence.Reduce(0);
            AssertSequence(1);
            Assert.AreEqual(1, sequence.Size);
        }
        [TestMethod]
        public void ReduceTest4() {
            sequence[0] = Bit.One;
            sequence[1] = Bit.One;
            sequence[4] = Bit.One;

            sequence.Reduce(1);
            sequence[5] = Bit.One;
            AssertSequence(1, 1, 0, 0, 0, 1);
            Assert.AreEqual(6, sequence.Size);
        }
        [TestMethod]
        public void FromStringGuardCase1Test() {
            AssertHelper.Throws<ArgumentNullException>(() => BitSequence.FromString(null));
        }
        [TestMethod]
        public void FromStringGuardCase2Test() {
            AssertHelper.Throws<ArgumentException>(() => BitSequence.FromString("01210"));
        }
        [TestMethod]
        public void FromStringGuardCase3Test() {
            AssertHelper.Throws<ArgumentException>(() => BitSequence.FromString("01-"));
        }
        [TestMethod]
        public void FromStringTest1() {
            sequence = BitSequence.FromString(string.Empty);
            Assert.IsNotNull(sequence);
            Assert.AreEqual(0, sequence.Size);
        }
        [TestMethod]
        public void FromStringTest2() {
            sequence = BitSequence.FromString("100100011100");
            Assert.IsNotNull(sequence);
            AssertSequence(sequence, 1, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0);
        }
        [TestMethod]
        public void EqualsSimpleTest() {
            sequence = new BitSequence();
            Assert.IsFalse(sequence.Equals(null));
            Assert.IsTrue(sequence.Equals(sequence));
            Assert.IsTrue(sequence.Equals(new BitSequence()));
        }
        [TestMethod]
        public void EqualsTest() {
            sequence[0] = Bit.One;
            sequence[1] = Bit.Zero;
            sequence[2] = Bit.Zero;
            sequence[3] = Bit.One;
            sequence[4] = Bit.One;

            BitSequence otherSequence = new BitSequence();
            otherSequence[0] = Bit.One;
            otherSequence[1] = Bit.One;
            Assert.IsFalse(sequence.Equals(otherSequence));

            otherSequence[0] = Bit.One;
            otherSequence[1] = Bit.Zero;
            otherSequence[2] = Bit.Zero;
            otherSequence[3] = Bit.One;
            otherSequence[4] = Bit.One;
            Assert.IsTrue(sequence.Equals(otherSequence));
        }

        private void AssertSequence(params int[] bits) {
            AssertSequence(sequence, bits);
        }
        static void AssertSequence(BitSequence sequence, params int[] bits) {
            int[] actual = sequence.Select(x => x == Bit.Zero ? 0 : 1).ToArray();
            CollectionAssert.AreEqual(bits, actual);
        }
    }
}
