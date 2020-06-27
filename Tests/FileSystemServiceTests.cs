#if DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileArchiver.Base;
using FileArchiver.Helpers;
using FileArchiver.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class FileSystemServiceTests {
        DefaultFileSystemService service;
        readonly TestIPlatformService platform = new TestIPlatformService();

        [TestInitialize]
        public void SetUp() {
            this.service = new DefaultFileSystemService(platform);
        }
        [TestMethod]
        public void EnumFileSystemEntriesGuardCase1Test() {
            AssertHelper.Throws<ArgumentNullException>(() => service.EnumFileSystemEntries(null).ToArray());
        }
        [TestMethod]
        public void EnumFileSystemEntriesGuardCase2Test() {
            AssertHelper.Throws<ArgumentException>(() => service.EnumFileSystemEntries(string.Empty).ToArray());
        }
        [TestMethod]
        public void EnumFileSystemEntriesGuardCase3Test() {
            platform.DirectoryExists = _ => false;
            platform.FileExists = _ => false;

            AssertHelper.Throws<ArgumentException>(() => {
                service.EnumFileSystemEntries(@"C:\").ToArray();
            });
        }

        [TestMethod]
        public void EnumFilesInEmptyDirectoryTest() {
            platform.DirectoryExists = _ => true;

            FileSystemEntry[] expected = {
                new FileSystemEntry(FileSystemEntryType.Directory, @"Root", @"C:\Root\")
            };
            AssertHelper.AreEqual(expected, service.EnumFileSystemEntries(@"C:\Root\"));
        }
        [TestMethod]
        public void EnumSingleFileTest() {
            platform.FileExists = _ => true;

            FileSystemEntry[] expected = {
                new FileSystemEntry(FileSystemEntryType.File, "file.dat", @"C:\Root\file.dat")
            };
            AssertHelper.AreEqual(expected, service.EnumFileSystemEntries(@"C:\Root\file.dat"));
        }
        [TestMethod]
        public void EnumFileSystemEntriesTest1() {
            platform.FileExists = x => x.EndsWith(".dat");
            platform.DirectoryExists = x => x == @"C:\Root\";

            platform.EnumFilesFunc = _ => new[] {
                @"C:\Root\file1.dat",
                @"C:\Root\file2.dat",
                @"C:\Root\file3.dat",
            };

            FileSystemEntry[] expected = {
                new FileSystemEntry(FileSystemEntryType.Directory, @"Root", @"C:\Root\"),
                new FileSystemEntry(FileSystemEntryType.File, "file1.dat", @"C:\Root\file1.dat"),
                new FileSystemEntry(FileSystemEntryType.File, "file2.dat", @"C:\Root\file2.dat"),
                new FileSystemEntry(FileSystemEntryType.File, "file3.dat", @"C:\Root\file3.dat"),
            };
            AssertHelper.AreEqual(expected, service.EnumFileSystemEntries(@"C:\Root\"));
        }

        [TestMethod]
        public void EnumFileSystemEntriesTest2() {
            platform.DirectoryExists = DefaultDirectoryExists;
            platform.FileExists = DefaultFileExists;

            Dictionary<string, string[]> dirs = new Dictionary<string, string[]> {
                {@"C:\Root\", new[] {@"D1\", @"D3\"}},
                {@"C:\Root\D1\", new[] {@"D2\"}},
                {@"C:\Root\D3\", new[] {@"D4\"}},
                {@"C:\Root\D3\D4\", new[] {@"D5\", @"D6\"}},
                {@"C:\Root\D1\D2\", new string[0]},
                {@"C:\Root\D3\D4\D5\", new string[0]},
                {@"C:\Root\D3\D4\D6\", new string[0]},
            };
            Dictionary<string, string[]> files = new Dictionary<string, string[]> {
                {@"C:\Root\", new[] {"F1.dat", "F2.dat"}},
                {@"C:\Root\D1\", new[] {"F1.dat", "F2.dat", "F3.dat"}},
                {@"C:\Root\D3\", new[] {"F1.dat"}},
                {@"C:\Root\D3\D4\", new[] {"F1.dat", "F2.dat"}},
                {@"C:\Root\D1\D2\", new string[0]},
                {@"C:\Root\D3\D4\D5\", new[] {"F1.dat", "F2.dat"}},
                {@"C:\Root\D3\D4\D6\", new[] {"F1.dat"}},
            };
            platform.EnumFilesFunc = path => files[path].Select(x => path + x);
            platform.EnumDirectoriesFunc = path => dirs[path].Select(x => path + x);

            IReadOnlyList<FileSystemEntry> expected = FileSystemEntryHelper.CreateListBuilder()
                .AddDirectory(@"C:\Root\")
                .AddFiles("F1.dat", "F2.dat")
                .AddDirectory(@"C:\Root\D1\")
                .AddFiles("F1.dat", "F2.dat", "F3.dat")
                .AddDirectory(@"C:\Root\D3\")
                .AddFile("F1.dat")
                .AddDirectory(@"C:\Root\D1\D2\")
                .AddDirectory(@"C:\Root\D3\D4\")
                .AddFiles("F1.dat", "F2.dat")
                .AddDirectory(@"C:\Root\D3\D4\D5\")
                .AddFiles("F1.dat", "F2.dat")
                .AddDirectory(@"C:\Root\D3\D4\D6\")
                .AddFile("F1.dat").GetList();
            AssertHelper.AreEqual(expected, service.EnumFileSystemEntries(@"C:\Root\"));
        }
        [TestMethod]
        public void EnumFileSystemEntriesTest3() {
            platform.DirectoryExists = DefaultDirectoryExists;
            platform.FileExists = DefaultFileExists;

            Dictionary<string, string[]> dirs = new Dictionary<string, string[]> {
                {@"F:\", new[] {@"D1\", @"D2\"}},
                {@"F:\D1\", new[] {@"D3\", @"D4\"}},
                {@"F:\D2\", new string[0]},
                {@"F:\D1\D3\", new string[0]},
                {@"F:\D1\D4\", new[] {@"D5\"}},
                {@"F:\D1\D4\D5\", new[] {@"D6\"}},
                {@"F:\D1\D4\D5\D6\", new string[0]},
            };
            Dictionary<string, string[]> files = new Dictionary<string, string[]> {
                {@"F:\", new[] {"F1.dat"}},
                {@"F:\D1\", new[] {"F1.dat", "F2.dat"}},
                {@"F:\D2\", new string[0]},
                {@"F:\D1\D3\", new[] {"F3.dat"}},
                {@"F:\D1\D4\", new string[0]},
                {@"F:\D1\D4\D5\", new[] {"F2.dat"}},
                {@"F:\D1\D4\D5\D6\", new[] {"F1.dat", "F2.dat", "F3.dat"}},
            };
            platform.EnumFilesFunc = path => files[path].Select(x => path + x);
            platform.EnumDirectoriesFunc = path => dirs[path].Select(x => path + x);

            IReadOnlyList<FileSystemEntry> expected = FileSystemEntryHelper.CreateListBuilder()
                .AddDirectory(@"F:\")
                .AddFile("F1.dat")
                .AddDirectory(@"F:\D1\")
                .AddFiles("F1.dat", "F2.dat")
                .AddDirectory(@"F:\D2\")
                .AddDirectory(@"F:\D1\D3\")
                .AddFile("F3.dat")
                .AddDirectory(@"F:\D1\D4\")
                .AddDirectory(@"F:\D1\D4\D5\")
                .AddFile("F2.dat")
                .AddDirectory(@"F:\D1\D4\D5\D6\")
                .AddFiles("F1.dat", "F2.dat", "F3.dat").GetList();
            AssertHelper.AreEqual(expected, service.EnumFileSystemEntries(@"F:\"));
        }
        static readonly Predicate<string> DefaultFileExists = path => {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            return path.EndsWith(".dat");
        };
        static readonly Predicate<string> DefaultDirectoryExists = path => {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            return path[path.Length - 1] == '\\';
        };
    }
}
#endif