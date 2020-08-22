#if DEBUG

using FileArchiver.HuffmanCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class WeightsTableTests {
        WeightsTable weightsTable;

        [TestInitialize]
        public void OnInitialize() {
            this.weightsTable = new WeightsTable();
        }
        [TestMethod]
        public void DefaultsTest() {
            Assert.AreEqual(0, weightsTable.Size);
        }
        [TestMethod]
        public void SizeTest() {
            weightsTable.TrackSymbol(0);
            Assert.AreEqual(1, weightsTable.Size);
            weightsTable.TrackSymbol(0);
            Assert.AreEqual(1, weightsTable.Size);
            WeightsTableExtensions.TrackSymbol(weightsTable, 0, 20);
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
#endif