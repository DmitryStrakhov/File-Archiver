using System;
using FileArchiver.Core.HuffmanCore;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture]
    public class WeightsTableTests {
        WeightsTable weightsTable;

        [SetUp]
        public void OnInitialize() {
            this.weightsTable = new WeightsTable();
        }
        [Test]
        public void DefaultsTest() {
            Assert.AreEqual(0, weightsTable.Size);
        }
        [Test]
        public void SizeTest() {
            weightsTable.TrackSymbol(0);
            Assert.AreEqual(1, weightsTable.Size);
            weightsTable.TrackSymbol(0);
            Assert.AreEqual(1, weightsTable.Size);
            weightsTable.TrackSymbol(0, 20);
            Assert.AreEqual(1, weightsTable.Size);

            weightsTable.TrackSymbol(0x10);
            weightsTable.TrackSymbol(0x20);
            weightsTable.TrackSymbol(0x30);
            Assert.AreEqual(4, weightsTable.Size);
            weightsTable.TrackSymbol(0x20);
            Assert.AreEqual(4, weightsTable.Size);
        }
    }
}
