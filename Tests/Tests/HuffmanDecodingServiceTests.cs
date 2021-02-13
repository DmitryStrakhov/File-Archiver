using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileArchiver.Core.Base;
using FileArchiver.Core.Format;
using FileArchiver.Core.HuffmanCore;
using FileArchiver.Core.Parsers;
using FileArchiver.Core.Services;
using NUnit.Framework;

namespace FileArchiver.Tests {

    #region TestIStreamParser

    public class TestIStreamParser : IStreamParser {
        public TestIStreamParser(TestIFileDecoder decoder) {
            this.Decoder = decoder;
        }

        #region IStreamParser

        BootstrapSegment IStreamParser.ParseWeightsTable(IDecodingInputStream stream) {
            Trace += "->ParseWeightsTable;";
            return new BootstrapSegment(new WeightsTable());
        }
        DirectorySegment IStreamParser.ParseDirectory(IDecodingInputStream stream) {
            Trace += "->ParseDirectory;";
            return new DirectorySegment(DirectoryName, Cardinality);
        }
        FileSegment IStreamParser.ParseFile(IDecodingInputStream stream, WeightsTable weightsTable) {
            Trace += "->ParseFile;";
            return new FileSegment(FileName, Decoder);
        }

        #endregion

        public TestIFileDecoder Decoder { get; }
        public string DirectoryName { get; } = "DirectoryName";
        public int Cardinality { get; set; }
        public string FileName { get; } = "FileName";
        public string Trace { get; set; } = string.Empty;
    }

    #endregion

    #region TestIFileDecoder

    public class TestIFileDecoder : IFileDecoder {

        #region IFileDecoder

        void IFileDecoder.Decode(IDecodingOutputStream outputStream, IProgressHandler progress) {
            Trace += "->Decode;";
        }

        #endregion

        public string Trace { get; set; } = string.Empty;
    }

    #endregion

    #region IProgress<DecodingProgressInfo>

    public class TestDecodingIProgress : IProgress<CodingProgressInfo> {
        readonly List<int> valueList;
        readonly List<string> messageList;

        public TestDecodingIProgress() {
            this.valueList = new List<int>(128);
            this.messageList = new List<string>(128);
        }

        #region IProgress<DecodingProgressInfo>

        void IProgress<CodingProgressInfo>.Report(CodingProgressInfo value) {
            valueList.Add(value.Value);
            messageList.Add(value.StatusMessage);
        }

        #endregion

        public IReadOnlyCollection<int> ValueList { get { return valueList; } }
        public IReadOnlyCollection<string> MessageList { get { return messageList; } }
    }

    #endregion

    
    [TestFixture]
    public class DefaultHuffmanDecodingServiceCtorTests {
        [Test]
        public void GuardTest1() {
            Assert.Throws<ArgumentNullException>(() => new DefaultHuffmanDecodingService(null, new TestIStreamParser(new TestIFileDecoder())));
        }
        [Test]
        public void GuardTest2() {
            Assert.Throws<ArgumentNullException>(() => new DefaultHuffmanDecodingService(new TestIPlatformService(), null));
        }
    }

    
    [TestFixture]
    public class HuffmanDecodingServiceBehaviorTests {
        DefaultHuffmanDecodingService service;
        TestIPlatformService platform;
        TestIStreamParser streamParser;
        TestIFileDecoder fileDecoder;

        [SetUp]
        public void SetUp() {
            this.platform = new TestIPlatformService {
                WriteFileFunc = _ => new MemoryStream()
            };
            this.fileDecoder = new TestIFileDecoder();
            this.streamParser = new TestIStreamParser(fileDecoder);
            this.service = new DefaultHuffmanDecodingService(platform, streamParser);
        }
        [Test]
        public async Task DecodeTest1() {
            platform.ReadFileFunc = x => new MemoryStream(new byte[0]);

            streamParser.Trace = string.Empty;
            await service.DecodeAsync(@"C:\Input.archive", @"C:\Output\", null);
            Assert.AreEqual(string.Empty, streamParser.Trace);
        }
        [Test]
        public async Task DecodeTest2() {
            byte[] data = { 0x2, 0x0 };
            platform.ReadFileFunc = x => new MemoryStream(data);

            streamParser.Trace = string.Empty;
            await service.DecodeAsync(@"C:\Input.archive", @"C:\Output\", null);
            Assert.AreEqual("->ParseWeightsTable;->ParseFile;", streamParser.Trace);
        }
        [Test]
        public async Task DecodeTest3() {
            byte[] data = { 0x2, 0x1, 0x0, 0x0, 0x1, 0x0, 0x1 };
            platform.ReadFileFunc = x => new MemoryStream(data);

            streamParser.Cardinality = 2;
            streamParser.Trace = string.Empty;
            await service.DecodeAsync(@"C:\Input.archive", @"C:\Output\", null);
            string expected = "->ParseWeightsTable;" +
                              "->ParseDirectory;" +
                              "->ParseFile;" +
                              "->ParseFile;" +
                              "->ParseDirectory;" +
                              "->ParseFile;" +
                              "->ParseDirectory;";
            Assert.AreEqual(expected, streamParser.Trace);
        }
        [Test]
        public void DecodeGuardTest() {
            byte[] data = { 0x1, 0x0, 0x0, 0x1 };
            platform.ReadFileFunc = x => new MemoryStream(data);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DecodeAsync(@"C:\Input.archive", @"C:\Output\", null));
        }
    }


    [TestFixture]
    public class HuffmanDecodingServiceTests {
        DefaultHuffmanDecodingService service;
        TestIPlatformService platform;

        [SetUp]
        public void SetUp() {
            this.platform = new TestIPlatformService();
            this.service = new DefaultHuffmanDecodingService(platform, new DefaultStreamParser());
        }
        [Test]
        public void DecodeGuardCase1Test() {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.DecodeAsync(null, @"C:\Dir\", null));
        }
        [Test]
        public void DecodeGuardCase2Test() {
            Assert.ThrowsAsync<ArgumentException>(async () => await service.DecodeAsync(string.Empty, @"C:\Dir\", null));
        }
        [Test]
        public void DecodeGuardCase3Test() {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.DecodeAsync(@"C:\File.dat", null, null));
        }
        [Test]
        public void DecodeGuardCase4Test() {
            Assert.ThrowsAsync<ArgumentException>(async () => await service.DecodeAsync(@"C:\File.dat", string.Empty, null));
        }
        
        [Test]
        public async Task DecodeEmptyStreamTest() {
            platform.ReadFileFunc = x => new BufferBuilder()
                .AddByte(0x2)
                .AddInt(0x0).GetStream();

            platform.Trace = string.Empty;
            platform.WriteFileFunc += x => new MemoryStream();
            await service.DecodeAsync(@"C:\InputFile.dat", @"C:\Root\", null);
            Assert.AreEqual(@"->ReadFile(C:\InputFile.dat)", platform.Trace);
        }
        [Test]
        public async Task DecodeEmptyDirectoryTest() {
            platform.ReadFileFunc = x => new BufferBuilder()
                .AddByte(0x2)
                .AddInt(0x0)
                .AddByte(0x1)
                .AddInt(0x6)
                .AddString("dir")
                .AddInt(0x0).GetStream();

            platform.Trace = string.Empty;
            await service.DecodeAsync(@"C:\InputFile.dat", @"C:\Root\", null);
            Assert.AreEqual(@"->ReadFile(C:\InputFile.dat)->CreateDirectory(C:\Root\dir)", platform.Trace);
        }
        [Test]
        public async Task DecodeEmptyFileTest() {
            platform.ReadFileFunc = x => new BufferBuilder()
                .AddByte(0x2)
                .AddInt(0x0)
                .AddByte(0x0)
                .AddInt(0x10)
                .AddString("file.dat")
                .AddLong(0x0).GetStream();

            MemoryStream dataStream = new MemoryStream();
            platform.WriteFileFunc += x => dataStream;
            platform.Trace = string.Empty;
            
            await service.DecodeAsync(@"C:\InputFile.dat", @"C:\Root\", null);
            Assert.AreEqual(@"->ReadFile(C:\InputFile.dat)->WriteFile(C:\Root\file.dat)", platform.Trace);
            AssertStreamIsEmpty(dataStream);
        }
        [Test]
        public async Task DecodeFileTest() {
            platform.ReadFileFunc = x => new BufferBuilder()
                .AddByte(0x2)
                .AddInt(0x24)
                .AddByte(0x1)
                .AddLong(0x1)
                .AddByte(0x2)
                .AddLong(0x2)
                .AddByte(0x3)
                .AddLong(0x4)
                .AddByte(0x4)
                .AddLong(0x8)
                .AddByte(0x0)
                .AddInt(0x10)
                .AddString("file.dat")
                .AddLong(0x19)
                .AddByte(0x20)
                .AddByte(0x55)
                .AddByte(0xFF)
                .AddByte(0x01).GetStream();

            MemoryStream dataStream = new MemoryStream();
            platform.WriteFileFunc += x => dataStream;
            platform.Trace = string.Empty;

            await service.DecodeAsync(@"C:\InputFile.dat", @"C:\Root\", null);
            Assert.AreEqual(@"->ReadFile(C:\InputFile.dat)->WriteFile(C:\Root\file.dat)", platform.Trace);
            AssertStream(dataStream, 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4);
        }
        [Test]
        public async Task DecodeDirectoryTest1() {
            platform.ReadFileFunc = x => new BufferBuilder()
                .AddByte(0x2)
                .AddInt(0x24)
                .AddByte(0x1)
                .AddLong(0x2)
                .AddByte(0x3)
                .AddLong(0x1)
                .AddByte(0x9)
                .AddLong(0x5)
                .AddByte(0x15)
                .AddLong(0x4)
                .AddByte(0x1)
                .AddInt(0x6)
                .AddString("dir")
                .AddInt(2)
                .AddByte(0x0)
                .AddInt(0xC)
                .AddString("f1.dat")
                .AddLong(0x9)
                .AddByte(0x7D)
                .AddByte(0x0)
                .AddByte(0x0)
                .AddInt(0xC)
                .AddString("f2.dat")
                .AddLong(0x6)
                .AddByte(0x33)
                .AddByte(0x1)
                .AddInt(0xE)
                .AddString("subdir1")
                .AddInt(0x0)
                .AddByte(0x0)
                .AddInt(0xC)
                .AddString("f3.dat")
                .AddLong(0x0)
                .AddByte(0x1)
                .AddInt(0xE)
                .AddString("subdir2")
                .AddInt(0x0)
                .AddByte(0x0)
                .AddInt(0xC)
                .AddString("f4.dat")
                .AddLong(0x7)
                .AddByte(0x29).GetStream();

            const string f1 = @"C:\Root\dir\f1.dat";
            const string f2 = @"C:\Root\dir\f2.dat";
            const string f3 = @"C:\Root\dir\subdir1\f3.dat";
            const string f4 = @"C:\Root\dir\subdir2\f4.dat";
            Dictionary<string, MemoryStream> streams = new Dictionary<string, MemoryStream>() {
                {f1, new MemoryStream()},
                {f2, new MemoryStream()},
                {f3, new MemoryStream()},
                {f4, new MemoryStream()},
            };
            platform.WriteFileFunc += x => streams[x];
            platform.Trace = string.Empty;
            await service.DecodeAsync(@"C:\InputFile.dat", @"C:\Root\", null);
            
            string expectedTrace = @"->ReadFile(C:\InputFile.dat)" +
                @"->CreateDirectory(C:\Root\dir)" + 
                @"->WriteFile(C:\Root\dir\f1.dat)" + 
                @"->WriteFile(C:\Root\dir\f2.dat)" + 
                @"->CreateDirectory(C:\Root\dir\subdir1)" + 
                @"->WriteFile(C:\Root\dir\subdir1\f3.dat)" + 
                @"->CreateDirectory(C:\Root\dir\subdir2)" +
                @"->WriteFile(C:\Root\dir\subdir2\f4.dat)";
            Assert.AreEqual(expectedTrace, platform.Trace);

            AssertStream(streams[f1], 0x1, 0x15, 0x15, 0x9, 0x9);
            AssertStream(streams[f2], 0x15, 0x9, 0x9, 0x15);
            AssertStreamIsEmpty(streams[f3]);
            AssertStream(streams[f4], 0x3, 0x1, 0x9);
        }
        [Test]
        public async Task DecodeDirectoryTest2() {
            platform.ReadFileFunc = x => new BufferBuilder()
                .AddByte(0x2)
                .AddInt(0x00)
                .AddByte(0x1)
                .AddInt(0x4)
                .AddString("d0")
                .AddInt(2)
                .AddByte(0x1)
                .AddInt(0x4)
                .AddString("d1")
                .AddInt(0)
                .AddByte(0x1)
                .AddInt(0x4)
                .AddString("d2")
                .AddInt(2)
                .AddByte(0x1)
                .AddInt(0x4)
                .AddString("d3")
                .AddInt(2)
                .AddByte(0x1)
                .AddInt(0x4)
                .AddString("d4")
                .AddInt(1)
                .AddByte(0x1)
                .AddInt(0x4)
                .AddString("d5")
                .AddInt(0)
                .AddByte(0x1)
                .AddInt(0x4)
                .AddString("d6")
                .AddInt(2)
                .AddByte(0x1)
                .AddInt(0x4)
                .AddString("d7")
                .AddInt(0)
                .AddByte(0x1)
                .AddInt(0x4)
                .AddString("d8")
                .AddInt(0)
                .AddByte(0x1)
                .AddInt(0x4)
                .AddString("d9")
                .AddInt(0)
                .GetStream();

            platform.Trace = string.Empty;
            await service.DecodeAsync(@"C:\InputFile.dat", @"C:\Root\", null);
            
            string expectedTrace = @"->ReadFile(C:\InputFile.dat)" +
                @"->CreateDirectory(C:\Root\d0)" + 
                @"->CreateDirectory(C:\Root\d0\d1)" + 
                @"->CreateDirectory(C:\Root\d0\d2)" + 
                @"->CreateDirectory(C:\Root\d0\d2\d3)" + 
                @"->CreateDirectory(C:\Root\d0\d2\d4)" + 
                @"->CreateDirectory(C:\Root\d0\d2\d3\d5)" + 
                @"->CreateDirectory(C:\Root\d0\d2\d3\d6)" + 
                @"->CreateDirectory(C:\Root\d0\d2\d4\d7)" + 
                @"->CreateDirectory(C:\Root\d0\d2\d3\d6\d8)" + 
                @"->CreateDirectory(C:\Root\d0\d2\d3\d6\d9)";
            Assert.AreEqual(expectedTrace, platform.Trace);
        }
        [Test]
        public async Task DecodingProgressTest1() {
            platform.ReadFileFunc = x => new BufferBuilder()
                .AddByte(0x2)
                .AddInt(0x9)
                .AddByte(0x0)
                .AddLong(0x1)
                .AddByte(0x1)
                .AddInt(0x6)
                .AddString("dir")
                .AddInt(2)
                .AddByte(0x0)
                .AddInt(0xC)
                .AddString("f1.dat")
                .AddLong(0x8)
                .AddByte(0x0)
                .AddByte(0x0)
                .AddInt(0xC)
                .AddString("f2.dat")
                .AddLong(0x8)
                .AddByte(0x0).GetStream();

            const string f1 = @"C:\Root\dir\f1.dat";
            const string f2 = @"C:\Root\dir\f2.dat";
            Dictionary<string, MemoryStream> streams = new Dictionary<string, MemoryStream>() {
                {f1, new MemoryStream()},
                {f2, new MemoryStream()},
            };
            platform.WriteFileFunc += x => streams[x];

            TestDecodingIProgress progress = new TestDecodingIProgress();
            await service.DecodeAsync(@"C:\InputFile.dat", @"C:\Root\", progress);
            CollectionAssert.AreEqual(new[] {0, 100}, progress.ValueList);
            CollectionAssert.AreEqual(new[] {"[Start]", "[Finish]"}, progress.MessageList);
        }
        [Test]
        public async Task DecodingProgressTest2() {
            platform.ReadFileFunc = x => new BufferBuilder()
                .AddByte(0x2)
                .AddInt(0x9)
                .AddByte(0x0)
                .AddLong(0x1)
                .AddByte(0x1)
                .AddInt(0x6)
                .AddString("dir")
                .AddInt(2)
                .AddByte(0x0)
                .AddInt(0xC)
                .AddString("f1.dat")
                .AddLong(200 * 1024 * 8) // 200Kb
                .AddByte(0x0, 200 * 1024)
                .AddByte(0x0)
                .AddInt(0xC)
                .AddString("f2.dat")
                .AddLong(300 * 1024 * 8) // 300Kb
                .AddByte(0x0, 300 * 1024).GetStream();

            const string f1 = @"C:\Root\dir\f1.dat";
            const string f2 = @"C:\Root\dir\f2.dat";
            Dictionary<string, MemoryStream> streams = new Dictionary<string, MemoryStream>() {
                {f1, new MemoryStream()},
                {f2, new MemoryStream()},
            };
            platform.WriteFileFunc += x => streams[x];

            TestDecodingIProgress progress = new TestDecodingIProgress();
            await service.DecodeAsync(@"C:\InputFile.dat", @"C:\Root\", progress);
            int[] expectedValues = {
                0, 25, 51, 76, 100
            };
            string[] expectedMessages = {
                "[Start]",
                @"C:\Root\dir\f1.dat",
                @"C:\Root\dir\f2.dat",
                @"C:\Root\dir\f2.dat",
                "[Finish]"
            };
            CollectionAssert.AreEqual(expectedValues, progress.ValueList);
            CollectionAssert.AreEqual(expectedMessages, progress.MessageList);
        }

        private void AssertStream(MemoryStream stream, params byte[] expected) {
            if(stream == null) throw new AssertionException(nameof(stream) + " is null");
            CollectionAssert.AreEqual(expected, stream.ToArray());
        }
        private void AssertStreamIsEmpty(MemoryStream stream) {
            if(stream == null || stream.ToArray().Length != 0)
                throw new AssertionException(nameof(stream) + " is not empty");
        }
    }
}
