using System;
using System.Collections.Generic;
using FileArchiver.Core.DataStructures;
using FileArchiver.Core.Format;
using FileArchiver.Core.HuffmanCore;
using FileArchiver.Core.Parsers;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture]
    public class StreamParserTests {
        DefaultStreamParser streamParser;

        [SetUp]
        public void SetUp() {
            this.streamParser = new DefaultStreamParser();
        }

        [Test]
        public void ParseWeightsTableGuardTest() {
            Assert.Throws<ArgumentNullException>(() => streamParser.ParseWeightsTable(null));
        }
        [Test]
        public void ParseWeightsTableTest1() {
            IReadOnlyList<Bit> data = BitListHelper.CreateBuilder()
                .AddInt(0x0).BitList;

            BootstrapSegment segment = streamParser.ParseWeightsTable(new TestIDecodingInputStream(data));
            Assert.IsNotNull(segment);
            WeightsTable weightsTable = segment.WeightsTable;
            Assert.IsNotNull(weightsTable);
            Assert.AreEqual(0, weightsTable.Size);
        }
        [Test]
        public void ParseWeightsTableTest2() {
            IReadOnlyList<Bit> data = BitListHelper.CreateBuilder()
                .AddInt(0x9)
                .AddByte(0x45)
                .AddLong(0x33).BitList;

            BootstrapSegment segment = streamParser.ParseWeightsTable(new TestIDecodingInputStream(data));
            Assert.IsNotNull(segment);
            WeightsTable weightsTable = segment.WeightsTable;
            Assert.IsNotNull(weightsTable);

            WeightedSymbol[] expectedSymbols = {
                new WeightedSymbol(0x45, 0x33)
            };
            Assert.AreEqual(expectedSymbols, weightsTable);
        }
        [Test]
        public void ParseWeightsTableTest3() {
            IReadOnlyList<Bit> data = BitListHelper.CreateBuilder()
                .AddInt(0x2D)
                .AddByte(0x1)
                .AddLong(0x1)
                .AddByte(0x2)
                .AddLong(0xFFF)
                .AddByte(0xF)
                .AddLong(0x5)
                .AddByte(0xAF)
                .AddLong(0xFFA)
                .AddByte(0xFF)
                .AddLong(0x1).BitList;

            BootstrapSegment segment = streamParser.ParseWeightsTable(new TestIDecodingInputStream(data));
            Assert.IsNotNull(segment);
            WeightsTable weightsTable = segment.WeightsTable;
            Assert.IsNotNull(weightsTable);

            WeightedSymbol[] expectedSymbols = {
                new WeightedSymbol(0x1, 0x1),
                new WeightedSymbol(0x2, 0xFFF),
                new WeightedSymbol(0xF, 0x5),
                new WeightedSymbol(0xAF, 0xFFA),
                new WeightedSymbol(0xFF, 1),
            };
            Assert.AreEqual(expectedSymbols, weightsTable);
        }

        [Test]
        public void ParseDirectoryGuardTest() {
            Assert.Throws<ArgumentNullException>(() => streamParser.ParseDirectory(null));
        }
        [Test]
        public void ParseDirectoryTest1() {
            IReadOnlyList<Bit> data = BitListHelper.CreateBuilder()
                .AddInt(0x2)
                .AddChar('X')
                .AddInt(0x0).BitList;

            DirectorySegment segment = streamParser.ParseDirectory(new TestIDecodingInputStream(data));
            Assert.IsNotNull(segment);
            Assert.AreEqual("X", segment.Name);
            Assert.AreEqual(0, segment.Cardinality);
        }
        [Test]
        public void ParseDirectoryTest2() {
            IReadOnlyList<Bit> data = BitListHelper.CreateBuilder()
                .AddInt(0x12)
                .AddString("Directory")
                .AddInt(0x5).BitList;

            DirectorySegment segment = streamParser.ParseDirectory(new TestIDecodingInputStream(data));
            Assert.IsNotNull(segment);
            Assert.AreEqual("Directory", segment.Name);
            Assert.AreEqual(5, segment.Cardinality);
        }

        [Test]
        public void ParseFileGuardCase1Test() {
            Assert.Throws<ArgumentNullException>(() => streamParser.ParseFile(null, new WeightsTable()));
        }
        [Test]
        public void ParseFileGuardCase2Test() {
            Assert.Throws<ArgumentNullException>(() => streamParser.ParseFile(new TestIDecodingInputStream(new Bit[0]),  null));
        }
        [Test]
        public void ParseFileTest1() {
            IReadOnlyList<Bit> data = BitListHelper.CreateBuilder()
                .AddInt(0x10)
                .AddChar('f')
                .AddChar('i')
                .AddChar('l')
                .AddChar('e')
                .AddChar('.')
                .AddChar('b')
                .AddChar('i')
                .AddChar('n')
                .AddLong(0x0).BitList;

            FileSegment segment = streamParser.ParseFile(new TestIDecodingInputStream(data), new WeightsTable());
            Assert.IsNotNull(segment);
            Assert.AreEqual("file.bin", segment.Name);
            Assert.IsNull(segment.Path);

            IFileDecoder decoder = segment.FileDecoder;
            Assert.IsNotNull(decoder);
            TestIDecodingOutputStream outputStream = new TestIDecodingOutputStream();
            decoder.Decode(outputStream, null);
            CollectionAssert.IsEmpty(outputStream.ByteList);
        }
        [Test]
        public void ParseFileTest2() {
            IReadOnlyList<Bit> data = BitListHelper.CreateBuilder()
                .AddInt(0x10)
                .AddChar('d')
                .AddChar('a')
                .AddChar('t')
                .AddChar('a')
                .AddChar('.')
                .AddChar('b')
                .AddChar('i')
                .AddChar('n')
                .AddLong(0x19)
                .AddByte(0x20)
                .AddByte(0x55)
                .AddByte(0xFF)
                .AddByte(0x01).BitList;

            WeightsTable weightsTable = new WeightsTable();
            weightsTable.TrackSymbol(1, 1);
            weightsTable.TrackSymbol(2, 2);
            weightsTable.TrackSymbol(3, 4);
            weightsTable.TrackSymbol(4, 8);

            FileSegment segment = streamParser.ParseFile(new TestIDecodingInputStream(data), weightsTable);
            Assert.IsNotNull(segment);
            Assert.AreEqual("data.bin", segment.Name);
            Assert.IsNull(segment.Path);

            IFileDecoder decoder = segment.FileDecoder;
            Assert.IsNotNull(decoder);
            TestIDecodingOutputStream outputStream = new TestIDecodingOutputStream();
            decoder.Decode(outputStream, null);
            byte[] expectedData = {1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4};
            Assert.AreEqual(expectedData, outputStream.ByteList);
        }
    }
}
