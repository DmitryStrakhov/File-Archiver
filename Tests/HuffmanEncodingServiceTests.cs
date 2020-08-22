#if DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Base;
using FileArchiver.Builders;
using FileArchiver.HuffmanCore;
using FileArchiver.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    
    #region TestStreamBuilder

    class TestStreamBuilder : IStreamBuilder {
        public TestStreamBuilder() {
        }

        #region IStreamBuilder

        void IStreamBuilder.Initialize(IPlatformService platform, HuffmanEncoder encoder, EncodingToken token, IEncodingOutputStream stream) {
            Trace += "->Initialize;";
        }
        void IStreamBuilder.AddWeightsTable(WeightsTable weightsTable) {
            Trace += "->AddWeightsTable;";
        }
        void IStreamBuilder.AddDirectory(string name) {
            Trace += $"->AddDirectory({name});";
        }
        void IStreamBuilder.AddFile(string name, string path) {
            Trace += $"->AddFile({name});";
        }
        
        #endregion

        public string Trace { get; set; } = string.Empty;
    }

    #endregion


    [TestClass]
    public class HuffmanEncodingServiceCtorTests {
        [TestMethod]
        public void GuardTest1() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                new DefaultHuffmanEncodingService(null, new TestIPlatformService(), new TestStreamBuilder());
            });
        }
        [TestMethod]
        public void GuardTest2() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                new DefaultHuffmanEncodingService(new TestIFileSystemService(), null, new TestStreamBuilder());
            });
        }
        [TestMethod]
        public void GuardTest3() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                new DefaultHuffmanEncodingService(new TestIFileSystemService(), new TestIPlatformService(), null);
            });
        }
    }


    [TestClass]
    public class HuffmanEncodingServiceBehaviorTests {
        DefaultHuffmanEncodingService service;
        TestIPlatformService platformService;
        TestIFileSystemService fileSystemService;
        TestStreamBuilder streamBuilder;

        [TestInitialize]
        public void SetUp() {
            this.fileSystemService = new TestIFileSystemService();

            this.platformService = new TestIPlatformService {
                ReadFileFunc = x => new MemoryStream(),
                WriteFileFunc = x => new MemoryStream()
            };
            this.streamBuilder = new TestStreamBuilder();
            this.service = new DefaultHuffmanEncodingService(fileSystemService, platformService, streamBuilder);
        }
        
        [TestMethod]
        public void EncodeTest1() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => new FileSystemEntry[0];

            streamBuilder.Trace = string.Empty;
            service.Encode(@"C:\dir\", "file.dat");
            Assert.AreEqual("->Initialize;->AddWeightsTable;", streamBuilder.Trace);
        }
        [TestMethod]
        public void EncodeTest2() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => new[] {
                new FileSystemEntry(FileSystemEntryType.File, "file1.dat", @"C:\file1.dat")
            };
            
            streamBuilder.Trace = string.Empty;
            service.Encode(@"C:\file1.dat", @"C:\file2.dat");
            Assert.AreEqual("->Initialize;->AddWeightsTable;->AddFile(file1.dat);", streamBuilder.Trace);
        }
        [TestMethod]
        public void EncodeTest3() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => new[] {
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

    [TestClass]
    public class HuffmanEncodingServiceTests {
        DefaultHuffmanEncodingService service;
        TestIPlatformService platformService;
        TestIFileSystemService fileSystemService;
        MemoryStream outputStream;

        [TestInitialize]
        public void SetUp() {
            this.fileSystemService = new TestIFileSystemService();
            this.outputStream = new MemoryStream();

            this.platformService = new TestIPlatformService {
                WriteFileFunc = x => outputStream
            };
            this.service = new DefaultHuffmanEncodingService(fileSystemService, platformService, new DefaultStreamBuilder());
        }
        
        [TestMethod]
        public void EncodeGuardCase1Test() {
            AssertHelper.Throws<ArgumentNullException>(() => service.Encode(null, "file"));
        }
        [TestMethod]
        public void EncodeGuardCase2Test() {
            AssertHelper.Throws<ArgumentException>(() => service.Encode(string.Empty, "file"));
        }
        [TestMethod]
        public void EncodeGuardCase3Test() {
            AssertHelper.Throws<ArgumentNullException>(() => service.Encode("file", null));
        }
        [TestMethod]
        public void EncodeGuardCase4Test() {
            AssertHelper.Throws<ArgumentException>(() => service.Encode("file", string.Empty));
        }

        [TestMethod]
        public void EncodeEmptyDirectoryTest() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.Directory, "dir", @"C:\dir\")};
            };

            service.Encode(@"C:\dir\", @"C:\outputFile.dat");
            byte[] expected = new BufferBuilder()
                .AddByte(0x2)
                .AddInt(0x0)
                .AddByte(0x1)
                .AddInt(0x6)
                .AddString("dir").GetData();
            CollectionAssert.AreEqual(expected, outputStream.ToArray());
        }
        [TestMethod]
        public void EncodeEmptyFileTest() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.File, "file.dat", @"C:\file.dat")};
            };
            platformService.ReadFileFunc += x => new MemoryStream();

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
        [TestMethod]
        public void EncodeFileTest() {
            byte[] data = {1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4};

            fileSystemService.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.File, "file.dat", @"C:\file.dat")};
            };
            platformService.ReadFileFunc += x => new MemoryStream(data);

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
        [TestMethod]
        public void EncodeDirectoryTest() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => {
                return new[] {
                    new FileSystemEntry(FileSystemEntryType.Directory, "dir", @"C:\dir\"),
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
            platformService.ReadFileFunc = x => new MemoryStream(data[x]);

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
                .AddByte(0x0)
                .AddInt(0xC)
                .AddString("f3.dat")
                .AddLong(0x0)
                .AddByte(0x1)
                .AddInt(0xE)
                .AddString("subdir2")
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
#endif