using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Builders;
using FileArchiver.DataStructures;
using FileArchiver.Format;
using FileArchiver.HuffmanCore;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture]
    public class StreamBuilderTests {
        DefaultStreamBuilder builder;

        [SetUp]
        public void SetUp() {
            this.builder = new DefaultStreamBuilder();
        }
        [Test]
        public void InitializeGuardCase1Test() {
            Assert.Throws<ArgumentNullException>(() => {
                builder.Initialize(null, EmptyEncodingToken.Instance,  new TestIEncodingOutputStream());
            });
        }
        [Test]
        public void InitializeGuardCase2Test() {
            Assert.Throws<ArgumentNullException>(() => {
                builder.Initialize(new TestIPlatformService(), null, new TestIEncodingOutputStream());
            });
        }
        [Test]
        public void InitializeGuardCase3Test() {
            Assert.Throws<ArgumentNullException>(() => {
                builder.Initialize(new TestIPlatformService(), EmptyEncodingToken.Instance, null);
            });
        }
        [Test]
        public void AddWeightsTableGuardTest() {
            Assert.Throws<ArgumentNullException>(() => builder.AddWeightsTable(null));
        }
        
        [Test]
        public void AddWeightsTableTest1() {
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            builder.Initialize(new TestIPlatformService(), EmptyEncodingToken.Instance, stream);

            builder.AddWeightsTable(new BootstrapSegment(new WeightsTable()));
            IReadOnlyList<Bit> bitList = BitListHelper.CreateBuilder()
                .AddByte(0x2)
                .AddInt(0x0).BitList;
            Assert.AreEqual(bitList, stream.BitList);
        }
        [Test]
        public void AddWeightsTableTest2() {
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            builder.Initialize(new TestIPlatformService(), EmptyEncodingToken.Instance, stream);

            WeightsTable weightsTable = new WeightsTable();
            weightsTable.TrackSymbol(symbol: 0x45, frequency: 0x33);

            builder.AddWeightsTable(new BootstrapSegment(weightsTable));
            IReadOnlyList<Bit> bitList = BitListHelper.CreateBuilder()
                .AddByte(0x2)
                .AddInt(0x9)
                .AddByte(0x45)
                .AddLong(0x33).BitList;
            Assert.AreEqual(bitList, stream.BitList);
        }
        [Test]
        public void AddWeightsTableTest3() {
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            builder.Initialize(new TestIPlatformService(), EmptyEncodingToken.Instance, stream);

            WeightsTable weightsTable = new WeightsTable();
            weightsTable.TrackSymbol(0x1);
            weightsTable.TrackSymbol(symbol: 0xF, frequency: 0x5);
            weightsTable.TrackSymbol(symbol: 0x2, frequency: 0xFFF);
            weightsTable.TrackSymbol(symbol: 0xAF, frequency: 0xFFA);
            weightsTable.TrackSymbol(0xFF);
            
            builder.AddWeightsTable(new BootstrapSegment(weightsTable));
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
            Assert.AreEqual(bitList, stream.BitList);
        }

        [Test]
        public void AddDirectoryGuardCase1Test() {
            Assert.Throws<ArgumentNullException>(() => builder.AddDirectory(null));
        }
        [Test]
        public void AddDirectoryGuardCase2Test() {
            Assert.Throws<ArgumentException>(() => builder.AddDirectory(new DirectorySegment(string.Empty, 0)));
        }
        [Test]
        public void AddDirectoryTest1() {
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            builder.Initialize(new TestIPlatformService(), EmptyEncodingToken.Instance, stream);
            
            builder.AddDirectory(new DirectorySegment("X", 0));
            IReadOnlyList<Bit> bitList = BitListHelper.CreateBuilder()
                .AddByte(0x1)
                .AddInt(0x2)
                .AddChar('X')
                .AddInt(0x0).BitList;
            Assert.AreEqual(bitList, stream.BitList);
        }
        [Test]
        public void AddDirectoryTest2() {
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            builder.Initialize(new TestIPlatformService(), EmptyEncodingToken.Instance, stream);
            
            builder.AddDirectory(new DirectorySegment("Directory", 3));
            IReadOnlyList<Bit> bitList = BitListHelper.CreateBuilder()
                .AddByte(0x1)
                .AddInt(0x12)
                .AddString("Directory")
                .AddInt(0x3).BitList;
            Assert.AreEqual(bitList, stream.BitList);
        }

        [Test]
        public void AddFileGuardCase1Test() {
            Assert.Throws<ArgumentNullException>(() => builder.AddFile(new FileSegment(null, "path")));
        }
        [Test]
        public void AddFileGuardCase2Test() {
            Assert.Throws<ArgumentNullException>(() => builder.AddFile(new FileSegment("name", (string)null)));
        }
        [Test]
        public void AddFileGuardCase3Test() {
            Assert.Throws<ArgumentException>(() => builder.AddFile(new FileSegment(string.Empty, "path")));
        }
        [Test]
        public void AddFileGuardCase4Test() {
            Assert.Throws<ArgumentException>(() => builder.AddFile(new FileSegment("name", string.Empty)));
        }
        [Test]
        public void AddFileTest1() {
            TestIPlatformService platform = new TestIPlatformService {
                ReadFileFunc = x => new MemoryStream()
            };
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            builder.Initialize(platform, EmptyEncodingToken.Instance, stream);
            
            builder.AddFile(new FileSegment(@"file.bin", @"C:\file.bin"));
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
            Assert.AreEqual(bitList, stream.BitList);
        }
        [Test]
        public void AddFileTest2() {
            byte[] data = { 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4 };

            TestIPlatformService platform = new TestIPlatformService {
                ReadFileFunc = x => new MemoryStream(data)
            };
            TestIEncodingOutputStream stream = new TestIEncodingOutputStream();
            HuffmanEncoder encoder = new HuffmanEncoder();
            builder.Initialize(platform, CreateEncodingToken(encoder, data), stream);

            builder.AddFile(new FileSegment(@"data.bin", @"C:\data.bin"));
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
            Assert.AreEqual(bitList, stream.BitList);
        }
        private EncodingToken CreateEncodingToken(HuffmanEncoder encoder, byte[] data) {
            using(TestIEncodingInputStream inputStream = new TestIEncodingInputStream(data)) {
                return encoder.CreateEncodingToken(inputStream);
            }
        }
    }
}
