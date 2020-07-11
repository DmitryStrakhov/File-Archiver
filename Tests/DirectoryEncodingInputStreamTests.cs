#if DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Base;
using FileArchiver.FileCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    #region TestIFileSystemService

    class TestIFileSystemService : IFileSystemService {
        public TestIFileSystemService() {
        }

        IEnumerable<FileSystemEntry> IFileSystemService.EnumFileSystemEntries(string path) {
            return EnumFileSystemEntriesFunc?.Invoke(path) ?? throw new InvalidOperationException();;
        }
        public Func<string, IEnumerable<FileSystemEntry>> EnumFileSystemEntriesFunc { get; set; }
    }

    #endregion

    [TestClass]
    public class DirectoryEncodingInputStreamTests {
        TestIFileSystemService fileSystemService;
        TestIPlatformService platform;

        [TestInitialize]
        public void SetUp() {
            this.fileSystemService = new TestIFileSystemService();
            this.platform = new TestIPlatformService();
        }
        [TestMethod]
        public void CtorGuardCase1Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                new DirectoryEncodingInputStream(null, new TestIFileSystemService(), new TestIPlatformService());
            });
        }
        [TestMethod]
        public void CtorGuardCase2Test() {
            AssertHelper.Throws<ArgumentException>(() => {
                new DirectoryEncodingInputStream(string.Empty, new TestIFileSystemService(), new TestIPlatformService());
            });
        }
        [TestMethod]
        public void CtorGuardCase3Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                new DirectoryEncodingInputStream("dir", null, new TestIPlatformService());
            });
        }
        [TestMethod]
        public void CtorGuardCase4Test() {
            AssertHelper.Throws<ArgumentNullException>(() => {
                new DirectoryEncodingInputStream("dir", new TestIFileSystemService(), null);
            });
        }
        [TestMethod]
        public void ReadSymbolTest1() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => new FileSystemEntry[0];

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir", fileSystemService, platform)) {
                Assert.IsFalse(stream.ReadSymbol(out byte _));
                Assert.IsFalse(stream.ReadSymbol(out byte _));
            }
        }
        [TestMethod]
        public void ReadSymbolTest2() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.File, "file.dat", @"C:\dir\file.dat") };
            };

            Dictionary<string, byte[]> data = new Dictionary<string, byte[]> {
                {@"C:\dir\file.dat", new byte[] {0x1, 0x2, 0x3, 0x7}}
            };
            platform.OpenFileFunc = x => new MemoryStream(data[x]);

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir\file.dat", fileSystemService, platform)) {
                byte symbol;
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x1, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x2, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x3, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x7, symbol);
                Assert.IsFalse(stream.ReadSymbol(out symbol));
            }
        }
        [TestMethod]
        public void ReadSymbolTest3() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.File, "file.dat", @"C:\dir\file.dat") };
            };

            Dictionary<string, byte[]> data = new Dictionary<string, byte[]> {
                {@"C:\dir\file.dat", new byte[] {0x11, 0x22, 0x33}}
            };
            platform.OpenFileFunc = x => new MemoryStream(data[x]);

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir", fileSystemService, platform)) {
                byte symbol;
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x11, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x22, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x33, symbol);
                Assert.IsFalse(stream.ReadSymbol(out symbol));
                Assert.IsFalse(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x00, symbol);
            }
        }
        [TestMethod]
        public void ReadSymbolTest4() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => {
                return new[] {
                    new FileSystemEntry(FileSystemEntryType.File, "file1.dat", @"C:\dir\file1.dat"),
                    new FileSystemEntry(FileSystemEntryType.Directory, "subdir1", @"C:\dir\subdir1"),
                    new FileSystemEntry(FileSystemEntryType.File, "file2.dat", @"C:\dir\file2.dat"),
                    new FileSystemEntry(FileSystemEntryType.Directory, "subdir2", @"C:\dir\subdir2"),
                    new FileSystemEntry(FileSystemEntryType.File, "file3.dat", @"C:\dir\file3.dat"),
                };
            };

            Dictionary<string, byte[]> data = new Dictionary<string, byte[]> {
                {@"C:\dir\file1.dat", new byte[] {0x11, 0x33}},
                {@"C:\dir\file2.dat", new byte[] {0xEA, 0x22, 0x33}},
                {@"C:\dir\file3.dat", new byte[] {0x13}},
            };
            platform.OpenFileFunc = x => new MemoryStream(data[x]);

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir\", fileSystemService, platform)) {
                byte symbol;
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x11, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x33, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0xEA, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x22, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x33, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x13, symbol);
                Assert.IsFalse(stream.ReadSymbol(out symbol));
            }
        }
        [TestMethod]
        public void ReadSymbolTest5() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => EnumFiles(3);

            Dictionary<string, byte[]> data = new Dictionary<string, byte[]> {
                {@"C:\dir\file1.dat", new byte[] {0x12, 0x33}},
                {@"C:\dir\file2.dat", new byte[] {0xEA, 0x33}},
                {@"C:\dir\file3.dat", new byte[] {0x13}},
            };
            platform.OpenFileFunc = x => new MemoryStream(data[x]);
            
            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir\", fileSystemService, platform)) {
                byte symbol;
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x12, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x33, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0xEA, symbol);
                stream.Reset();
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x12, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x33, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0xEA, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x33, symbol);
                Assert.IsTrue(stream.ReadSymbol(out symbol));
                Assert.AreEqual(0x13, symbol);
                Assert.IsFalse(stream.ReadSymbol(out symbol));
            }
        }

        private static IEnumerable<FileSystemEntry> EnumFiles(int count) {
            for(int n = 1; n <= count; n++) {
                string name = $"file{n.ToString()}.dat";
                yield return new FileSystemEntry(FileSystemEntryType.File, name, @"C:\dir\" + name);
            }
        }
    }
}

#endif