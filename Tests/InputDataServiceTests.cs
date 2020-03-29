#if DEBUG

using System;
using FileArchiver.Base;
using FileArchiver.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    #region IPlatformService

    public class TestIPlatformService : IPlatformService {
        public TestIPlatformService() {
        }

        #region IPlatformService

        bool IPlatformService.FileExists(string path) {
            return FileExists;
        }
        bool IPlatformService.FolderExists(string path) {
            return FolderExists;
        }

        #endregion

        public bool FileExists { get; set; }
        public bool FolderExists { get; set; }
    }

    #endregion


    [TestClass]
    public class InputDataServiceTests {
        DefaultInputDataService service;
        TestIPlatformService platformService;

        [TestInitialize]
        public void SetUp() {
            this.platformService = new TestIPlatformService();
            this.service = new DefaultInputDataService(platformService);
        }
        [TestCleanup]
        public void TearDown() {
            this.service = null;
        }
        [TestMethod]
        public void IsDataFileTest1() {
            platformService.FileExists = true;
            Assert.AreEqual(InputType.FileToEncode, service.GetInputType(@"C:\Temp\File.dat"));
            Assert.AreEqual(InputType.FileToEncode, service.GetInputType(@"D:\Text.txt"));
            Assert.AreEqual(InputType.FileToEncode, service.GetInputType(@"E:\File.html"));
        }
        [TestMethod]
        public void IsDataFileTest2() {
            platformService.FileExists = false;
            Assert.AreEqual(InputType.Unknown, service.GetInputType(@"C:\File.bin"));
        }
        [TestMethod]
        public void IsFolderTest1() {
            platformService.FolderExists = true;
            Assert.AreEqual(InputType.FolderToEncode, service.GetInputType(@"C:\Temp"));
            Assert.AreEqual(InputType.FolderToEncode, service.GetInputType(@"D:\"));
            Assert.AreEqual(InputType.FolderToEncode, service.GetInputType(@"D:"));
            Assert.AreEqual(InputType.FolderToEncode, service.GetInputType(@"E:\Folder1\Folder2"));
            Assert.AreEqual(InputType.FolderToEncode, service.GetInputType(@"E:\Folder1\Folder2\"));
        }
        [TestMethod]
        public void IsFolderTest2() {
            platformService.FolderExists = false;
            Assert.AreEqual(InputType.Unknown, service.GetInputType(@"C:\Temp\"));
        }
        [TestMethod]
        public void IsArchiveTest1() {
            platformService.FileExists = true;
            Assert.AreEqual(InputType.Archive, service.GetInputType(@"C:\File.archive"));
            Assert.AreEqual(InputType.Archive, service.GetInputType(@"E:\Folder\File.archive"));
        }
        [TestMethod]
        public void IsArchiveTest2() {
            platformService.FileExists = false;
            Assert.AreEqual(InputType.Unknown, service.GetInputType(@"C:\File.archive"));
        }
    }
}
#endif