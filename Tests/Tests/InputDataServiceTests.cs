using System;
using FileArchiver.Core.Base;
using FileArchiver.Core.Services;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture]
    public class InputDataServiceTests {
        DefaultInputDataService service;
        TestIPlatformService platform;

        [SetUp]
        public void SetUp() {
            this.platform = new TestIPlatformService();
            this.service = new DefaultInputDataService(platform);
        }
        [TearDown]
        public void TearDown() {
            this.service = null;
        }
        [Test]
        public void IsDataFileTest1() {
            platform.PathExists = _ => true;
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"C:\Temp\File.dat"));
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"D:\Text.txt"));
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"E:\File.html"));
        }
        [Test]
        public void IsDataFileTest2() {
            platform.PathExists = _ => false;
            Assert.AreEqual(InputCommand.Unknown, service.GetInputCommand(@"C:\File.bin"));
        }
        [Test]
        public void IsFolderTest1() {
            platform.PathExists = _ => true;
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"C:\Temp"));
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"D:\"));
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"D:"));
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"E:\Folder1\Folder2"));
            Assert.AreEqual(InputCommand.Encode, service.GetInputCommand(@"E:\Folder1\Folder2\"));
        }
        [Test]
        public void IsFolderTest2() {
            platform.PathExists = _ => false;
            Assert.AreEqual(InputCommand.Unknown, service.GetInputCommand(@"C:\Temp\"));
        }
        [Test]
        public void IsArchiveTest1() {
            platform.PathExists = _ => true;
            Assert.AreEqual(InputCommand.Decode, service.GetInputCommand(@"C:\File.archive"));
            Assert.AreEqual(InputCommand.Decode, service.GetInputCommand(@"E:\Folder\File.archive"));
        }
        [Test]
        public void IsArchiveTest2() {
            platform.PathExists = _ => false;
            Assert.AreEqual(InputCommand.Unknown, service.GetInputCommand(@"C:\File.archive"));
        }
    }
}
