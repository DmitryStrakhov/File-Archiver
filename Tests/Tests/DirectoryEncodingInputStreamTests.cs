using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Core.Base;
using FileArchiver.Core.FileCore;
using FileArchiver.Core.Helpers;
using NUnit.Framework;

namespace FileArchiver.Tests {
    #region TestIFileSystemService

    class TestIFileSystemService : IFileSystemService {
        public TestIFileSystemService() {
        }

        IEnumerable<FileSystemEntry> IFileSystemService.EnumFileSystemEntries(string path) {
            return EnumFileSystemEntriesFunc?.Invoke(path) ?? throw new InvalidOperationException();
        }
        public Func<string, IEnumerable<FileSystemEntry>> EnumFileSystemEntriesFunc { get; set; }
    }

    #endregion

    #region TestMemoryStream

    public class TestMemoryStream : MemoryStream {
        public TestMemoryStream(byte[] data, IList<TestMemoryStream> repository = null)
            : base(data) {
            Guard.IsNotNull(data, nameof(data));
            repository?.Add(this);
        }
        protected override void Dispose(bool disposing) {
            Trace += "->Dispose;";
            base.Dispose(disposing);
        }
        public string Trace { get; set; } = string.Empty;
    }

    #endregion

    [TestFixture]
    public class DirectoryEncodingInputStreamTests {
        TestIFileSystemService fileSystemService;
        TestIPlatformService platform;

        [SetUp]
        public void SetUp() {
            this.fileSystemService = new TestIFileSystemService();
            this.platform = new TestIPlatformService();
        }
        [Test]
        public void CtorGuardCase1Test() {
            Assert.Throws<ArgumentNullException>(() => {
                new DirectoryEncodingInputStream(null, new TestIFileSystemService(), new TestIPlatformService());
            });
        }
        [Test]
        public void CtorGuardCase2Test() {
            Assert.Throws<ArgumentException>(() => {
                new DirectoryEncodingInputStream(string.Empty, new TestIFileSystemService(), new TestIPlatformService());
            });
        }
        [Test]
        public void CtorGuardCase3Test() {
            Assert.Throws<ArgumentNullException>(() => {
                new DirectoryEncodingInputStream("dir", null, new TestIPlatformService());
            });
        }
        [Test]
        public void CtorGuardCase4Test() {
            Assert.Throws<ArgumentNullException>(() => {
                new DirectoryEncodingInputStream("dir", new TestIFileSystemService(), null);
            });
        }
        [Test]
        public void ReadSymbolTest1() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => new FileSystemEntry[0];

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir", fileSystemService, platform)) {
                Assert.IsFalse(stream.ReadSymbol(out byte _));
                Assert.IsFalse(stream.ReadSymbol(out byte _));
            }
        }
        [Test]
        public void ReadSymbolTest2() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.File, "file.dat", @"C:\dir\file.dat") };
            };

            Dictionary<string, byte[]> data = new Dictionary<string, byte[]> {
                {@"C:\dir\file.dat", new byte[] {0x1, 0x2, 0x3, 0x7}}
            };
            platform.ReadFileFunc = x => new MemoryStream(data[x]);

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir\file.dat", fileSystemService, platform)) {
                Assert.IsTrue(stream.ReadSymbol(out byte symbol));
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
        [Test]
        public void ReadSymbolTest3() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => {
                return new[] { new FileSystemEntry(FileSystemEntryType.File, "file.dat", @"C:\dir\file.dat") };
            };

            Dictionary<string, byte[]> data = new Dictionary<string, byte[]> {
                {@"C:\dir\file.dat", new byte[] {0x11, 0x22, 0x33}}
            };
            platform.ReadFileFunc = x => new MemoryStream(data[x]);

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir", fileSystemService, platform)) {
                Assert.IsTrue(stream.ReadSymbol(out byte symbol));
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
        [Test]
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
            platform.ReadFileFunc = x => new MemoryStream(data[x]);

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir\", fileSystemService, platform)) {
                Assert.IsTrue(stream.ReadSymbol(out byte symbol));
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
        [Test]
        public void ReadSymbolTest5() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => EnumFiles(3);

            Dictionary<string, byte[]> data = new Dictionary<string, byte[]> {
                {@"C:\dir\file1.dat", new byte[] {0x12, 0x33}},
                {@"C:\dir\file2.dat", new byte[] {0xEA, 0x33}},
                {@"C:\dir\file3.dat", new byte[] {0x13}},
            };
            platform.ReadFileFunc = x => new MemoryStream(data[x]);
            
            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir\", fileSystemService, platform)) {
                Assert.IsTrue(stream.ReadSymbol(out byte symbol));
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
        [Test]
        public void IsTraversedPropertyTest() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => EnumFiles(2);

            Dictionary<string, byte[]> data = new Dictionary<string, byte[]> {
                {@"C:\dir\file1.dat", new byte[] {0x12, 0x33}},
                {@"C:\dir\file2.dat", new byte[] {0xEA, 0x33}},
            };
            platform.ReadFileFunc = x => new MemoryStream(data[x]);

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir\", fileSystemService, platform)) {
                Assert.IsFalse(stream.IsTraversed);
                while(stream.ReadSymbol(out byte _)) {
                }
                Assert.IsTrue(stream.IsTraversed);
            }
        }
        [Test]
        public void SizeInBytesPropertyGuardTest() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => EnumFiles(2);

            Dictionary<string, byte[]> data = new Dictionary<string, byte[]> {
                {@"C:\dir\file1.dat", new byte[] {0x12, 0x33}},
                {@"C:\dir\file2.dat", new byte[] {0xEA, 0x33}},
            };
            platform.ReadFileFunc = x => new MemoryStream(data[x]);

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir\", fileSystemService, platform)) {
                Assert.Throws<InvalidOperationException>(() => { long size = stream.SizeInBytes; });
            }
        }
        [Test]
        public void SizeInBytesPropertyTest1() {
            fileSystemService.EnumFileSystemEntriesFunc = x => new FileSystemEntry[0];

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir\", fileSystemService, platform)) {
                while(stream.ReadSymbol(out byte _)) {
                }
                Assert.AreEqual(0, stream.SizeInBytes);
            }
        }
        [Test]
        public void SizeInBytesPropertyTest2() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => EnumFiles(3);

            Dictionary<string, byte[]> data = new Dictionary<string, byte[]> {
                {@"C:\dir\file1.dat", new byte[10 * 1024]},
                {@"C:\dir\file2.dat", new byte[12 * 1024]},
                {@"C:\dir\file3.dat", new byte[123]},
            };
            platform.ReadFileFunc = x => new MemoryStream(data[x]);

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir\", fileSystemService, platform)) {
                while(stream.ReadSymbol(out byte _)) {
                }
                Assert.AreEqual(22651, stream.SizeInBytes);
            }
        }
        [Test]
        public void DisposeTest() {
            fileSystemService.EnumFileSystemEntriesFunc = _ => {
                return new[] {
                    new FileSystemEntry(FileSystemEntryType.File, "file1.dat", @"C:\dir\file1.dat"),
                    new FileSystemEntry(FileSystemEntryType.File, "file2.dat", @"C:\dir\file2.dat"),
                    new FileSystemEntry(FileSystemEntryType.File, "file3.dat", @"C:\dir\file3.dat"),
                };
            };
            List<TestMemoryStream> streamList = new List<TestMemoryStream>(16);
            platform.ReadFileFunc = x => {
                return new TestMemoryStream(new byte[] {1}, streamList);
            };

            using(DirectoryEncodingInputStream stream = new DirectoryEncodingInputStream(@"C:\dir\", fileSystemService, platform)) {
                while(stream.ReadSymbol(out byte _)) { }
            }
            Assert.That(streamList, Is.All.Matches<TestMemoryStream>(x => x.Trace == "->Dispose;"));
        }

        private static IEnumerable<FileSystemEntry> EnumFiles(int count) {
            for(int n = 1; n <= count; n++) {
                string name = $"file{n.ToString()}.dat";
                yield return new FileSystemEntry(FileSystemEntryType.File, name, @"C:\dir\" + name);
            }
        }
    }
}
