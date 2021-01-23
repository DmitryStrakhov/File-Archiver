﻿using System;
using System.Text;
using System.Threading.Tasks;
using FileArchiver.Core.Base;
using FileArchiver.Core.ViewModel;
using NUnit.Framework;

namespace FileArchiver.Tests {
    #region ViewModel

    public class TestViewModel : MainViewModel, ITraceableObject {
        public TestViewModel() {
            FileSelectorService = new TestIFileSelectorService(this);
            FolderSelectorService = new TestIFolderSelectorService(this);
            InputDataService = new TestIInputDataService(this);
            EncodingService = new TestIHuffmanEncodingService(this);
            DecodingService = new TestIHuffmanDecodingService(this);
        }
        public override Task Run() {
            Trace += "Run;";
            return base.Run();
        }
        public void TraceMember(string name) {
            Trace += name + ";";
        }
        public string GetTrace() {
            return Trace;
        }
        public string Trace { get; set; } = string.Empty;
    }

    #endregion

    #region Services

    public class TestIFileSelectorService : TraceableObject, IFileSelectorService {
        public TestIFileSelectorService(ITraceableObject traceableObject) : base(traceableObject) {
        }

        #region IFileSelectorService

        string IFileSelectorService.GetSaveFile() {
            TraceMember();
            return FilePath;
        }

        #endregion

        public string FilePath { get; set; }
    }

    public class TestIFolderSelectorService : TraceableObject, IFolderSelectorService {
        public TestIFolderSelectorService(ITraceableObject traceableObject) : base(traceableObject) {
        }

        #region IFolderSelectorService

        string IFolderSelectorService.GetFolder() {
            TraceMember();
            return FolderPath;
        }

        #endregion

        public string FolderPath { get; set; }
    }

    public class TestIInputDataService : TraceableObject, IInputDataService {
        public TestIInputDataService(ITraceableObject traceableObject) : base(traceableObject) {
        }

        #region IInputDataService

        InputCommand IInputDataService.GetInputCommand(string path) {
            TraceMember();
            return InputCommand;
        }

        #endregion

        public InputCommand InputCommand { get; set; } = InputCommand.Unknown;
    }

    public class TestIHuffmanEncodingService : TraceableObject, IHuffmanEncodingService {
        public TestIHuffmanEncodingService(ITraceableObject traceableObject) : base(traceableObject) {
        }

        #region IHuffmanEncodingService

        bool IHuffmanEncodingService.Encode(string inputPath, string outputFile) {
            TraceMember();
            return true;
        }

        #endregion
    }

    public class TestIHuffmanDecodingService : TraceableObject, IHuffmanDecodingService {
        public TestIHuffmanDecodingService(ITraceableObject traceableObject) : base(traceableObject) {
        }

        #region IHuffmanDecodingService

        void IHuffmanDecodingService.Decode(string inputFile, string outputFolder) {
            TraceMember();
        }

        #endregion
    }

    #endregion

    [TestFixture]
    public class ViewModelTests {
        TestViewModel viewModel;
        TestIInputDataService inputDataService;
        TestIFileSelectorService fileSelectorService;
        TestIFolderSelectorService folderSelectorService;

        [SetUp]
        public void SetUp() {
            this.viewModel = new TestViewModel();
            this.inputDataService = (TestIInputDataService)viewModel.InputDataService;
            this.fileSelectorService = (TestIFileSelectorService)viewModel.FileSelectorService;
            this.folderSelectorService = (TestIFolderSelectorService)viewModel.FolderSelectorService;
        }
        [TearDown]
        public void TearDown() {
            this.viewModel = null;
            this.inputDataService = null;
        }
        [Test]
        public void DefaultsTest() {
            Assert.IsFalse(viewModel.CanRun());
        }
        [Test]
        public async Task EncodeFileTest1() {
            inputDataService.InputCommand = InputCommand.Encode;
            viewModel.Path = "Path";

            fileSelectorService.FilePath = @"C:\File.archive";
            await viewModel.Run();
            Assert.AreEqual("Run;GetInputCommand;GetSaveFile;Encode;", viewModel.Trace);
        }
        [Test]
        public async Task EncodeFileTest2() {
            inputDataService.InputCommand = InputCommand.Encode;
            viewModel.Path = "Path";

            fileSelectorService.FilePath = null;
            await viewModel.Run();
            Assert.AreEqual("Run;GetInputCommand;GetSaveFile;", viewModel.Trace);
        }
        [Test]
        public async Task EncodeFolderTest() {
            inputDataService.InputCommand = InputCommand.Encode;
            viewModel.Path = "Path";

            fileSelectorService.FilePath = @"C:\File.archive";
            await viewModel.Run();
            Assert.AreEqual("Run;GetInputCommand;GetSaveFile;Encode;", viewModel.Trace);
        }
        [Test]
        public async Task DecodeFileTest1() {
            inputDataService.InputCommand = InputCommand.Decode;
            viewModel.Path = "Path";

            folderSelectorService.FolderPath = @"C:\Folder";
            await viewModel.Run();
            Assert.AreEqual("Run;GetInputCommand;GetFolder;Decode;", viewModel.Trace);
        }
        [Test]
        public async Task DecodeFileTest2() {
            inputDataService.InputCommand = InputCommand.Decode;
            viewModel.Path = "Path";

            folderSelectorService.FolderPath = null;
            await viewModel.Run();
            Assert.AreEqual("Run;GetInputCommand;GetFolder;", viewModel.Trace);
        }
        [Test]
        public void InvalidPathTest() {
            inputDataService.InputCommand = InputCommand.Unknown;
            viewModel.Path = "Path";

            Assert.ThrowsAsync<Exception>(() => viewModel.Run());
            Assert.AreEqual("Run;GetInputCommand;", viewModel.Trace);
        }
        [Test]
        public void CanRunTest1() {
            inputDataService.InputCommand = InputCommand.Encode;
            viewModel.Path = @"C:\Temp\file.bin";
            Assert.IsTrue(viewModel.CanRun());
        }
        [Test]
        public void CanRunTest2() {
            inputDataService.InputCommand = InputCommand.Encode;
            viewModel.Path = @"D:\Temp\";
            Assert.IsTrue(viewModel.CanRun());
        }
        [Test]
        public void CanRunTest3() {
            inputDataService.InputCommand = InputCommand.Decode;
            viewModel.Path = @"E:\Temp\file.archive";
            Assert.IsTrue(viewModel.CanRun());
        }
        [Test]
        public void CanRunTest4() {
            inputDataService.InputCommand = InputCommand.Unknown;
            viewModel.Path = @"some path";
            Assert.IsFalse(viewModel.CanRun());
        }
    }
}
