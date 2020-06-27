#if DEBUG

using System;
using FileArchiver.Base;
using FileArchiver.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    [TestClass]
    public class InputDataServiceTests {
        DefaultInputDataService service;
        TestIPlatformService platform;

        [TestInitialize]
        public void SetUp() {
            this.platform = new TestIPlatformService();
            this.service = new DefaultInputDataService(platform);
        }
        [TestCleanup]
        public void TearDown() {
            this.service = null;
        }
        [TestMethod]
        public void IsDataFileTest1() {
            platform.PathExists = _ => true;
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"C:\Temp\File.dat"));
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"D:\Text.txt"));
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"E:\File.html"));
        }
        [TestMethod]
        public void IsDataFileTest2() {
            platform.PathExists = _ => false;
            Assert.AreEqual(InputCommand.Unknown, service.GetInputCommand(@"C:\File.bin"));
        }
        [TestMethod]
        public void IsFolderTest1() {
            platform.PathExists = _ => true;
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"C:\Temp"));
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"D:\"));
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"D:"));
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"E:\Folder1\Folder2"));
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"E:\Folder1\Folder2\"));
        }
        [TestMethod]
        public void IsFolderTest2() {
            platform.PathExists = _ => false;
            Assert.AreEqual(InputCommand.Unknown, service.GetInputCommand(@"C:\Temp\"));
        }
        [TestMethod]
        public void IsArchiveTest1() {
            platform.PathExists = _ => true;
            Assert.AreEqual(InputCommand.Decode, service.GetInputCommand(@"C:\File.archive"));
            Assert.AreEqual(InputCommand.Decode, service.GetInputCommand(@"E:\Folder\File.archive"));
        }
        [TestMethod]
        public void IsArchiveTest2() {
            platform.PathExists = _ => false;
            Assert.AreEqual(InputCommand.Unknown, service.GetInputCommand(@"C:\File.archive"));
        }
    }
}
#endif