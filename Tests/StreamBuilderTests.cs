#if DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Base;
using FileArchiver.Builders;
using FileArchiver.DataStructures;
using FileArchiver.HuffmanCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class StreamBuilderTests {
        DefaultStreamBuilder builder;

        [TestInitialize]
        public void SetUp() {
            this.builder = new DefaultStreamBuilder();
        }
        [TestMethod]
        public void InitializeGuardCase1Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                builder.Initialize(null, new HuffmanEncoder(), EmptyEncodingToken.Instance,  new TestIEncodingOutputStream());
            });
        }
        [TestMethod]
        public void InitializeGuardCase2Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                builder.Initialize(new TestIPlatformService(), null, EmptyEncodingToken.Instance, new TestIEncodingOutputStream());
            });
        }
        [TestMethod]
        public void InitializeGuardCase3Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                builder.Initialize(new TestIPlatformService(),  new HuffmanEncoder(), null, new TestIEncodingOutputStream());
            });
        }
        [TestMethod]
        public void InitializeGuardCase4Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                builder.Initialize(new TestIPlatformService(),  new HuffmanEncoder(), EmptyEncodingToken.Instance, null);
            });
        }
        [TestMethod]
        public void AddWeightsTableGuardTest() {
            AssertHelper.Throws<ArgumentNullException>(() => builder.AddWeightsTable(null));
        }
        
        [TestMethod]
        public void AddWeightsTableTest1() {
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            builder.Initialize(new TestIPlatformService(), new HuffmanEncoder(), EmptyEncodingToken.Instance, stream);

            builder.AddWeightsTable(new WeightsTable());
            IReadOnlyList<Bit> bitList = BitListHelper.CreateBuilder()
                .AddByte(0x2)
                .AddInt(0x0).BitList;
            AssertHelper.AreEqual(bitList, stream.BitList);
        }
        [TestMethod]
        public void AddWeightsTableTest2() {
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            builder.Initialize(new TestIPlatformService(), new HuffmanEncoder(), EmptyEncodingToken.Instance, stream);

            WeightsTable weightsTable = new WeightsTable();
            WeightsTableExtensions.TrackSymbol(weightsTable, symbol: 0x45, times: 0x33);

            builder.AddWeightsTable(weightsTable);
            IReadOnlyList<Bit> bitList = BitListHelper.CreateBuilder()
                .AddByte(0x2)
                .AddInt(0x9)
                .AddByte(0x45)
                .AddLong(0x33).BitList;
            AssertHelper.AreEqual(bitList, stream.BitList);
        }
        [TestMethod]
        public void AddWeightsTableTest3() {
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            builder.Initialize(new TestIPlatformService(), new HuffmanEncoder(), EmptyEncodingToken.Instance, stream);

            WeightsTable weightsTable = new WeightsTable();
            weightsTable.TrackSymbol(0x1);
            WeightsTableExtensions.TrackSymbol(weightsTable, symbol: 0xF, times: 0x5);
            WeightsTableExtensions.TrackSymbol(weightsTable, symbol: 0x2, times: 0xFFF);
            WeightsTableExtensions.TrackSymbol(weightsTable, symbol: 0xAF, times: 0xFFA);
            weightsTable.TrackSymbol(0xFF);
            
            builder.AddWeightsTable(weightsTable);
            IReadOnlyList<Bit> bitList = BitListHelper.CreateBuilder()
                .AddByte(0x2)
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
            AssertHelper.AreEqual(bitList, stream.BitList);
        }

        [TestMethod]
        public void AddDirectoryGuardCase1Test() {
            AssertHelper.Throws<ArgumentException>(() => builder.AddDirectory(null));
        }
        [TestMethod]
        public void AddDirectoryGuardCase2Test() {
            AssertHelper.Throws<ArgumentException>(() => builder.AddDirectory(string.Empty));
        }
        [TestMethod]
        public void AddDirectoryTest1() {
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            builder.Initialize(new TestIPlatformService(), new HuffmanEncoder(), EmptyEncodingToken.Instance, stream);
            
            builder.AddDirectory("X");
            IReadOnlyList<Bit> bitList = BitListHelper.CreateBuilder()
                .AddByte(0x1)
                .AddInt(0x2)
                .AddChar('X').BitList;
            AssertHelper.AreEqual(bitList, stream.BitList);
        }
        [TestMethod]
        public void AddDirectoryTest2() {
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            builder.Initialize(new TestIPlatformService(), new HuffmanEncoder(), EmptyEncodingToken.Instance, stream);
            
            builder.AddDirectory("Directory");
            IReadOnlyList<Bit> bitList = BitListHelper.CreateBuilder()
                .AddByte(0x1)
                .AddInt(0x12)
                .AddChar('D')
                .AddChar('i')
                .AddChar('r')
                .AddChar('e')
                .AddChar('c')
                .AddChar('t')
                .AddChar('o')
                .AddChar('r')
                .AddChar('y').BitList;
            AssertHelper.AreEqual(bitList, stream.BitList);
        }

        [TestMethod]
        public void AddFileGuardCase1Test() {
            AssertHelper.Throws<ArgumentNullException>(() => builder.AddFile(null, "path"));
        }
        [TestMethod]
        public void AddFileGuardCase2Test() {
            AssertHelper.Throws<ArgumentNullException>(() => builder.AddFile("name", null));
        }
        [TestMethod]
        public void AddFileGuardCase3Test() {
            AssertHelper.Throws<ArgumentException>(() => builder.AddFile(string.Empty, "path"));
        }
        [TestMethod]
        public void AddFileGuardCase4Test() {
            AssertHelper.Throws<ArgumentException>(() => builder.AddFile("name", string.Empty));
        }
        [TestMethod]
        public void AddFileTest1() {
            TestIPlatformService platform = new TestIPlatformService {
                ReadFileFunc = x => new MemoryStream()
            };
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            builder.Initialize(platform, new HuffmanEncoder(), EmptyEncodingToken.Instance, stream);
            
            builder.AddFile(@"file.bin", @"C:\file.bin");
            IReadOnlyList<Bit> bitList = BitListHelper.CreateBuilder()
                .AddByte(0x00)
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
            AssertHelper.AreEqual(bitList, stream.BitList);
        }
        [TestMethod]
        public void AddFileTest2() {
            byte[] data = { 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4 };

            TestIPlatformService platform = new TestIPlatformService {
                ReadFileFunc = x => new MemoryStream(data)
            };
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            HuffmanEncoder encoder = new HuffmanEncoder();
            builder.Initialize(platform, encoder, CreateEncodingToken(encoder, data), stream);

            builder.AddFile(@"data.bin", @"C:\data.bin");
            IReadOnlyList<Bit> bitList = BitListHelper.CreateBuilder()
                .AddByte(0x00)
                .AddInt(0x10)
                .AddChar('d')
                .AddChar('a')
                .AddChar('t')
                .AddChar('a')
                .AddChar('.')
                .AddChar('b')
                .AddChar('i')
                .AddChar('n')
                .AddLong(0x19)//bits
                .AddByte(0x20)
                .AddByte(0x55)
                .AddByte(0xFF)
                .AddByte(0x01).BitList;
            AssertHelper.AreEqual(bitList, stream.BitList);
        }
        private EncodingToken CreateEncodingToken(HuffmanEncoder encoder, byte[] data) {
            using(TestIEncodingInputStream inputStream = new TestIEncodingInputStream(data)) {
                return encoder.CreateEncodingToken(inputStream);
            }
        }
    }
}
#endif