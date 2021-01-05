using System;
using System.Collections.Generic;
using System.Text;
using FileArchiver.Base;
using FileArchiver.ViewModel;
using NUnit.Framework;


namespace FileArchiver.Tests {
    #region ViewModel

    public class TestViewModel : MainViewModel {
        public TestViewModel(ServiceFactory serviceFactory) : base(serviceFactory) {
        }
        public override void Run() {
            Trace += "Run;";
            base.Run();
        }
        public string Trace { get; set; } = string.Empty;
    }

    #endregion

    #region Factory

    public class TestServiceFactory : ServiceFactory, ITraceableObject {
        readonly StringBuilder traceBuilder;

        public TestServiceFactory() {
            this.traceBuilder = new StringBuilder(64);
            FileSelectorService = new TestIFileSelectorService(this);
            FolderSelectorService = new TestIFolderSelectorService(this);
            InputDataService = new TestIInputDataService(this);
            EncodingService = new TestIHuffmanEncodingService(this);
            DecodingService = new TestIHuffmanDecodingService(this);
        }
        public void TraceMember(string name) {
            traceBuilder.Append(name);
            traceBuilder.Append(";");
        }
        public string GetTrace() {
            return traceBuilder.ToString();
        }

        public override IFileSelectorService FileSelectorService { get; }
        public override IFolderSelectorService FolderSelectorService { get; }
        public override IInputDataService InputDataService { get; }
        public override IHuffmanEncodingService EncodingService { get; }
        public override IHuffmanDecodingService DecodingService { get; }
        public TestIFolderSelectorService TestFolderSelectorService => (TestIFolderSelectorService)FolderSelectorService;
        public TestIFileSelectorService TestFileSelectorService => (TestIFileSelectorService)FileSelectorService;
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
        TestServiceFactory serviceFactory;
        TestIInputDataService inputDataService;

        [SetUp]
        public void SetUp() {
            this.serviceFactory = new TestServiceFactory();
            this.inputDataService = (TestIInputDataService)serviceFactory.InputDataService;
            this.viewModel = new TestViewModel(serviceFactory);
        }
        [TearDown]
        public void TearDown() {
            this.serviceFactory = null;
            this.viewModel = null;
            this.inputDataService = null;
        }
        [Test]
        public void DefaultsTest() {
            Assert.IsFalse(viewModel.CanRun());
        }
        [Test]
        public void EncodeFileTest1() {
            inputDataService.InputCommand = InputCommand.Encode;
            viewModel.Path = "Path";

            serviceFactory.TestFileSelectorService.FilePath = @"C:\File.archive";
            viewModel.Run();
            Assert.AreEqual("Run;", viewModel.Trace);
            Assert.AreEqual("GetInputCommand;GetSaveFile;Encode;", serviceFactory.GetTrace());
        }
        [Test]
        public void EncodeFileTest2() {
            inputDataService.InputCommand = InputCommand.Encode;
            viewModel.Path = "Path";

            serviceFactory.TestFileSelectorService.FilePath = null;
            viewModel.Run();
            Assert.AreEqual("Run;", viewModel.Trace);
            Assert.AreEqual("GetInputCommand;GetSaveFile;", serviceFactory.GetTrace());
        }
        [Test]
        public void EncodeFolderTest() {
            inputDataService.InputCommand = InputCommand.Encode;
            viewModel.Path = "Path";

            serviceFactory.TestFileSelectorService.FilePath = @"C:\File.archive";
            viewModel.Run();
            Assert.AreEqual("Run;", viewModel.Trace);
            Assert.AreEqual("GetInputCommand;GetSaveFile;Encode;", serviceFactory.GetTrace());
        }
        [Test]
        public void DecodeFileTest1() {
            inputDataService.InputCommand = InputCommand.Decode;
            viewModel.Path = "Path";

            serviceFactory.TestFolderSelectorService.FolderPath = @"C:\Folder";
            viewModel.Run();
            Assert.AreEqual("Run;", viewModel.Trace);
            Assert.AreEqual("GetInputCommand;GetFolder;Decode;", serviceFactory.GetTrace());
        }
        [Test]
        public void DecodeFileTest2() {
            inputDataService.InputCommand = InputCommand.Decode;
            viewModel.Path = "Path";

            serviceFactory.TestFolderSelectorService.FolderPath = null;
            viewModel.Run();
            Assert.AreEqual("Run;", viewModel.Trace);
            Assert.AreEqual("GetInputCommand;GetFolder;", serviceFactory.GetTrace());
        }
        [Test]
        public void InvalidPathTest() {
            inputDataService.InputCommand = InputCommand.Unknown;
            viewModel.Path = "Path";

            Assert.Throws<Exception>(() => viewModel.Run());
            Assert.AreEqual("Run;", viewModel.Trace);
            Assert.AreEqual("GetInputCommand;", serviceFactory.GetTrace());
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
