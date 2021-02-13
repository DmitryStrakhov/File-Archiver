using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileArchiver.Core.Base;
using FileArchiver.Core.Builders;
using FileArchiver.Core.Format;
using FileArchiver.Core.HuffmanCore;
using FileArchiver.Core.Services;
using NUnit.Framework;

namespace FileArchiver.Tests {
    
    #region TestStreamBuilder

    class TestIStreamBuilder : IStreamBuilder {
        public TestIStreamBuilder() {
        }

        #region IStreamBuilder

        void IStreamBuilder.Initialize(IPlatformService platform, EncodingToken token, IEncodingOutputStream stream) {
            Trace += "->Initialize;";
        }
        void IStreamBuilder.AddWeightsTable(BootstrapSegment segment) {
            Trace += "->AddWeightsTable;";
        }
        void IStreamBuilder.AddDirectory(DirectorySegment segment) {
            Trace += $"->AddDirectory({segment.Name});";
        }
        void IStreamBuilder.AddFile(FileSegment segment, IProgressHandler progress) {
            Trace += $"->AddFile({segment.Name});";
        }
        
        #endregion

        public string Trace { get; set; } = string.Empty;
    }

    #endregion

    #region IProgress<EncodingProgressInfo>

    public class TestEncodingProgress : IProgress<CodingProgressInfo> {
        readonly List<int> valueList;
        readonly List<string> messageList;

        public TestEncodingProgress() {
            this.valueList = new List<int>(128);
            this.messageList = new List<string>(128);
        }

        #region IProgress<EncodingProgressInfo>

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
    public class HuffmanEncodingServiceCtorTests {
        [Test]
        public void GuardTest1() {
            Assert.Throws<ArgumentNullException>(() => {
                new DefaultHuffmanEncodingService(null, new TestIPlatformService(), new TestIStreamBuilder());
            });
        }
        [Test]
        public void GuardTest2() {
            Assert.Throws<ArgumentNullException>(() => {
                new DefaultHuffmanEncodingService(new TestIFileSystemService(), null, new TestIStreamBuilder());
            });
        }
        [Test]
        public void GuardTest3() {
            Assert.Throws<ArgumentNullException>(() => {
                new DefaultHuffmanEncodingService(new TestIFileSystemService(), new TestIPlatformService(), null);
            });
        }
    }


    [TestFixture]
    public class HuffmanEncodingServiceBehaviorTests {
        DefaultHuffmanEncodingService service;
        TestIPlatformService platform;
        TestIFileSystemService fileSystem;
        TestIStreamBuilder streamBuilder;

        [SetUp]
        public void SetUp() {
            this.fileSystem = new TestIFileSystemService();

            this.platform = new TestIPlatformService {
                ReadFileFunc = x => new MemoryStream(),
                WriteFileFunc = x => new MemoryStream()
            };
            this.streamBuilder = new TestIStreamBuilder();
            this.service = new DefaultHuffmanEncodingService(fileSystem, platform, streamBuilder);
        }
        
        [Test]
        public async Task EncodeTest1() {
            fileSystem.EnumFileSystemEntriesFunc = _ => new FileSystemEntry[0];

            streamBuilder.Trace = string.Empty;
            await service.EncodeAsync(@"C:\dir\", "file.dat", null);
            Assert.AreEqual("->Initialize;->AddWeightsTable;", streamBuilder.Trace);
        }
        [Test]
        public async Task EncodeTest2() {
            fileSystem.EnumFileSystemEntriesFunc = _ => new[] {
                new FileSystemEntry(FileSystemEntryType.File, "file1.dat", @"C:\file1.dat")
            };
            
            streamBuilder.Trace = string.Empty;
            await service.EncodeAsync(@"C:\file1.dat", @"C:\file2.dat", null);
            Assert.AreEqual("->Initialize;->AddWeightsTable;->AddFile(file1.dat);", streamBuilder.Trace);
        }
        [Test]
        public async Task EncodeTest3() {
            fileSystem.EnumFileSystemEntriesFunc = _ => new[] {
                new FileSystemEntry(FileSystemEntryType.File, "file1.dat", @"C:\dir\file1.dat"),
                new FileSystemEntry(FileSystemEntryType.File, "file2.dat", @"C:\dir\file2.dat"),
                new FileSystemEntry(FileSystemEntryType.Directory, "subdir1", @"C:\dir\subdir1"),
                new FileSystemEntry(FileSystemEntryType.File, "file3.dat", @"C:\dir\file3.dat"),
                new FileSystemEntry(FileSystemEntryType.Directory, "subdir2", @"C:\dir\subdir2"),
            };
            streamBuilder.Trace = string.Empty;

            string expectedTrace = "->Initialize;->AddWeightsTable;" +
                                   "->AddFile(file1.dat);" +
                                   "->AddFile(file2.dat);" +
                                   "->AddDirectory(subdir1);" +
                                   "->AddFile(file3.dat);" +
                                   "->AddDirectory(subdir2);";
            await service.EncodeAsync(@"C:\dir\", @"C:\file.dat", null);
            Assert.AreEqual(expectedTrace, streamBuilder.Trace);
        }
    }

    [TestFixture]
    public class HuffmanEncodingServiceTests {
        DefaultHuffmanEncodingService service;
        TestIPlatformService platform;
        TestIFileSystemService fileSystem;
        MemoryStream outputStream;

        [SetUp]
        public void SetUp() {
            this.fileSystem = new TestIFileSystemService();
            this.outputStream = new MemoryStream();

            this.platform = new TestIPlatformService {
                WriteFileFunc = x => outputStream
            };
            this.service = new DefaultHuffmanEncodingService(fileSystem, platform, new DefaultStreamBuilder());
        }
        
        [Test]
        public void EncodeGuardCase1Test() {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.EncodeAsync(null, "file", null));
        }
        [Test]
        public void EncodeGuardCase2Test() {
            Assert.ThrowsAsync<ArgumentException>(async () => await service.EncodeAsync(string.Empty, "file", null));
        }
        [Test]
        public void EncodeGuardCase3Test() {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.EncodeAsync("file", null, null));
        }
        [Test]
        public void EncodeGuardCase4Test() {
            Assert.ThrowsAsync<ArgumentException>(async () => await service.EncodeAsync("file", string.Empty, null));
        }

        [Test]
        public async Task EncodeEmptyDirectoryTest() {
            fileSystem.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.Directory, "dir", @"C:\dir\")};
            };

            await service.EncodeAsync(@"C:\dir\", @"C:\outputFile.dat", null);
            byte[] expected = new BufferBuilder()
                .AddByte(0x2)
                .AddInt(0x0)
                .AddByte(0x1)
                .AddInt(0x6)
                .AddString("dir")
                .AddInt(0x0).GetData();
            CollectionAssert.AreEqual(expected, outputStream.ToArray());
        }
        [Test]
        public async Task EncodeEmptyFileTest() {
            fileSystem.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.File, "file.dat", @"C:\file.dat")};
            };
            platform.ReadFileFunc += x => new MemoryStream();

            await service.EncodeAsync(@"C:\file.dat", @"C:\outputFile.dat", null);
            byte[] expected = new BufferBuilder()
                .AddByte(0x2)
                .AddInt(0x0)
                .AddByte(0x0)
                .AddInt(0x10)
                .AddString("file.dat")
                .AddLong(0x0).GetData();
            CollectionAssert.AreEqual(expected, outputStream.ToArray());
        }
        [Test]
        public async Task EncodeFileTest() {
            byte[] data = {1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4};

            fileSystem.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.File, "file.dat", @"C:\file.dat")};
            };
            platform.ReadFileFunc += x => new MemoryStream(data);

            await service.EncodeAsync(@"C:\file.dat", @"C:\outputFile.dat", null);
            byte[] expected = new BufferBuilder()
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
                .AddByte(0x01).GetData();
            CollectionAssert.AreEqual(expected, outputStream.ToArray());
        }
        [Test]
        public async Task EncodeDirectoryTest1() {
            fileSystem.EnumFileSystemEntriesFunc = _ => {
                return new[] {
                    new FileSystemEntry(FileSystemEntryType.Directory, "dir", @"C:\dir\", 2),
                    new FileSystemEntry(FileSystemEntryType.File, "f1.dat", @"C:\dir\f1.dat"),
                    new FileSystemEntry(FileSystemEntryType.File, "f2.dat", @"C:\dir\f2.dat"),
                    new FileSystemEntry(FileSystemEntryType.Directory, "subdir1", @"C:\dir\subdir1\"),
                    new FileSystemEntry(FileSystemEntryType.File, "f3.dat", @"C:\dir\subdir1\f3.dat"),
                    new FileSystemEntry(FileSystemEntryType.Directory, "subdir2", @"C:\dir\subdir2\"),
                    new FileSystemEntry(FileSystemEntryType.File, "f4.dat", @"C:\dir\subdir2\f4.dat"),
                };
            };

            Dictionary<string, byte[]> data = new Dictionary<string, byte[]> {
                {@"C:\dir\f1.dat", new byte[] {0x1, 0x15, 0x15, 0x9, 0x9}},
                {@"C:\dir\f2.dat", new byte[] {0x15, 0x9, 0x9, 0x15}},
                {@"C:\dir\subdir1\f3.dat", new byte[0]},
                {@"C:\dir\subdir2\f4.dat", new byte[] {0x3, 0x1, 0x9}},
            };
            platform.ReadFileFunc = x => new MemoryStream(data[x]);

            await service.EncodeAsync(@"C:\dir\", @"C:\outputFile.dat", null);
            byte[] expected = new BufferBuilder()
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
                .AddInt(0x2)
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
                .AddByte(0x29)
                .GetData();
            CollectionAssert.AreEqual(expected, outputStream.ToArray());
        }
        [Test]
        public async Task EncodingProgressTest1() {
            fileSystem.EnumFileSystemEntriesFunc = _ => {
                return new[] {
                    new FileSystemEntry(FileSystemEntryType.Directory, "dir", @"C:\dir\", 2),
                    new FileSystemEntry(FileSystemEntryType.File, "f1.dat", @"C:\dir\f1.dat"),
                    new FileSystemEntry(FileSystemEntryType.File, "f2.dat", @"C:\dir\f2.dat"),
                    new FileSystemEntry(FileSystemEntryType.File, "f3.dat", @"C:\dir\f3.dat"),
                };
            };
            platform.ReadFileFunc = x => new MemoryStream(new byte[0]);

            TestEncodingProgress progress = new TestEncodingProgress();
            await service.EncodeAsync(@"C:\dir\", @"C:\outputFile.dat", progress);
            CollectionAssert.AreEqual(new[]{-1, 0, 100}, progress.ValueList);
            CollectionAssert.AreEqual(new[]{"[Build encoding token]", "[Start]", "[Finish]"}, progress.MessageList);
        }
        [Test]
        public async Task EncodingProgressTest2() {
            fileSystem.EnumFileSystemEntriesFunc = _ => {
                return new[] {
                    new FileSystemEntry(FileSystemEntryType.Directory, "dir", @"C:\dir\", 2),
                    new FileSystemEntry(FileSystemEntryType.File, "f1.dat", @"C:\dir\f1.dat"),
                    new FileSystemEntry(FileSystemEntryType.File, "f2.dat", @"C:\dir\f2.dat"),
                    new FileSystemEntry(FileSystemEntryType.File, "f3.dat", @"C:\dir\f3.dat"),
                };
            };
            platform.ReadFileFunc = x => {
                switch(x) {
                    case @"C:\dir\f1.dat": return new MemoryStream(new byte[200 * 1024]);
                    case @"C:\dir\f2.dat": return new MemoryStream(new byte[300 * 1024]);
                    case @"C:\dir\f3.dat": return new MemoryStream(new byte[2 * 1024]);
                    default: throw new NotImplementedException();
                }
            };

            TestEncodingProgress progress = new TestEncodingProgress();
            await service.EncodeAsync(@"C:\dir\", @"C:\outputFile.dat", progress);
            int[] expectedValues = {
                -1, 0, 25, 50, 76, 100
            };
            string[] expectedMessages = {
                "[Build encoding token]",
                "[Start]",
                @"C:\dir\f1.dat",
                @"C:\dir\f2.dat",
                @"C:\dir\f2.dat",
                "[Finish]"
            };
            CollectionAssert.AreEqual(expectedValues, progress.ValueList);
            CollectionAssert.AreEqual(expectedMessages, progress.MessageList);
        }
    }
}
