using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Base;
using FileArchiver.Builders;
using FileArchiver.Format;
using FileArchiver.HuffmanCore;
using FileArchiver.Services;
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
        void IStreamBuilder.AddFile(FileSegment segment) {
            Trace += $"->AddFile({segment.Name});";
        }
        
        #endregion

        public string Trace { get; set; } = string.Empty;
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
        public void EncodeTest1() {
            fileSystem.EnumFileSystemEntriesFunc = _ => new FileSystemEntry[0];

            streamBuilder.Trace = string.Empty;
            service.Encode(@"C:\dir\", "file.dat");
            Assert.AreEqual("->Initialize;->AddWeightsTable;", streamBuilder.Trace);
        }
        [Test]
        public void EncodeTest2() {
            fileSystem.EnumFileSystemEntriesFunc = _ => new[] {
                new FileSystemEntry(FileSystemEntryType.File, "file1.dat", @"C:\file1.dat")
            };
            
            streamBuilder.Trace = string.Empty;
            service.Encode(@"C:\file1.dat", @"C:\file2.dat");
            Assert.AreEqual("->Initialize;->AddWeightsTable;->AddFile(file1.dat);", streamBuilder.Trace);
        }
        [Test]
        public void EncodeTest3() {
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
            service.Encode(@"C:\dir\", @"C:\file.dat");
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
            Assert.Throws<ArgumentNullException>(() => service.Encode(null, "file"));
        }
        [Test]
        public void EncodeGuardCase2Test() {
            Assert.Throws<ArgumentException>(() => service.Encode(string.Empty, "file"));
        }
        [Test]
        public void EncodeGuardCase3Test() {
            Assert.Throws<ArgumentNullException>(() => service.Encode("file", null));
        }
        [Test]
        public void EncodeGuardCase4Test() {
            Assert.Throws<ArgumentException>(() => service.Encode("file", string.Empty));
        }

        [Test]
        public void EncodeEmptyDirectoryTest() {
            fileSystem.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.Directory, "dir", @"C:\dir\")};
            };

            service.Encode(@"C:\dir\", @"C:\outputFile.dat");
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
        public void EncodeEmptyFileTest() {
            fileSystem.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.File, "file.dat", @"C:\file.dat")};
            };
            platform.ReadFileFunc += x => new MemoryStream();

            service.Encode(@"C:\file.dat", @"C:\outputFile.dat");
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
        public void EncodeFileTest() {
            byte[] data = {1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4};

            fileSystem.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.File, "file.dat", @"C:\file.dat")};
            };
            platform.ReadFileFunc += x => new MemoryStream(data);

            service.Encode(@"C:\file.dat", @"C:\outputFile.dat");
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
        public void EncodeDirectoryTest1() {
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

            service.Encode(@"C:\dir\", @"C:\outputFile.dat");
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
    }
}
