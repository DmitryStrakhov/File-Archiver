#if DEBUG

using System;
using System.Text;
using FileArchiver.Base;
using FileArchiver.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace FileArchiver.Tests {
    #region ViewModel

    public class TestViewModel : MainViewModel {
        public TestViewModel(ServiceFactory serviceFactory) : base(serviceFactory) {
        }
        internal override void Run() {
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

        InputType IInputDataService.GetInputType(string path) {
            TraceMember();
            return InputType;
        }

        #endregion

        public InputType InputType { get; set; } = InputType.Unknown;
    }

    public class TestIHuffmanEncodingService : TraceableObject, IHuffmanEncodingService {
        public TestIHuffmanEncodingService(ITraceableObject traceableObject) : base(traceableObject) {
        }

        #region IHuffmanEncodingService

        bool IHuffmanEncodingService.EncodeFile(string inputFile, string outputFile) {
            TraceMember();
            return true;
        }
        bool IHuffmanEncodingService.EncodeFolder(string inputFolder, string outputFile) {
            TraceMember();
            return true;
        }

        #endregion
    }

    public class TestIHuffmanDecodingService : TraceableObject, IHuffmanDecodingService {
        public TestIHuffmanDecodingService(ITraceableObject traceableObject) : base(traceableObject) {
        }

        #region IHuffmanDecodingService

        bool IHuffmanDecodingService.Decode(string inputFile, string outputFolder) {
            TraceMember();
            return true;
        }

        #endregion
    }

    #endregion

    [TestClass]
    public class ViewModelTests {
        TestViewModel viewModel;
        TestServiceFactory serviceFactory;
        TestIInputDataService inputDataService;

        [TestInitialize]
        public void SetUp() {
            this.serviceFactory = new TestServiceFactory();
            this.inputDataService = (TestIInputDataService)serviceFactory.InputDataService;
            this.viewModel = new TestViewModel(serviceFactory);
        }
        [TestCleanup]
        public void TearDown() {
            this.serviceFactory = null;
            this.viewModel = null;
            this.inputDataService = null;
        }
        [TestMethod]
        public void DefaultsTest() {
            Assert.IsFalse(viewModel.CanRun());
        }
        [TestMethod]
        public void EncodeFileTest() {
            inputDataService.InputType = InputType.FileToEncode;
            viewModel.Path = "Path";

            viewModel.Run();
            Assert.AreEqual("Run;", viewModel.Trace);
            Assert.AreEqual("GetInputType;GetSaveFile;EncodeFile;", serviceFactory.GetTrace());
        }
        [TestMethod]
        public void EncodeFolderTest() {
            inputDataService.InputType = InputType.FolderToEncode;
            viewModel.Path = "Path";

            viewModel.Run();
            Assert.AreEqual("Run;", viewModel.Trace);
            Assert.AreEqual("GetInputType;GetSaveFile;EncodeFolder;", serviceFactory.GetTrace());
        }
        [TestMethod]
        public void DecodeFileTest() {
            inputDataService.InputType = InputType.Archive;
            viewModel.Path = "Path";

            viewModel.Run();
            Assert.AreEqual("Run;", viewModel.Trace);
            Assert.AreEqual("GetInputType;GetFolder;Decode;", serviceFactory.GetTrace());
        }
        [TestMethod]
        public void InvalidPathTest() {
            inputDataService.InputType = InputType.Unknown;
            viewModel.Path = "Path";

            AssertHelper.Throws<Exception>(() => viewModel.Run());
            Assert.AreEqual("Run;", viewModel.Trace);
            Assert.AreEqual("GetInputType;", serviceFactory.GetTrace());
        }
        [TestMethod]
        public void CanRunTest1() {
            inputDataService.InputType = InputType.FileToEncode;
            viewModel.Path = @"C:\Temp\file.bin";
            Assert.IsTrue(viewModel.CanRun());
        }
        [TestMethod]
        public void CanRunTest2() {
            inputDataService.InputType = InputType.FolderToEncode;
            viewModel.Path = @"D:\Temp\";
            Assert.IsTrue(viewModel.CanRun());
        }
        [TestMethod]
        public void CanRunTest3() {
            inputDataService.InputType = InputType.Archive;
            viewModel.Path = @"E:\Temp\file.archive";
            Assert.IsTrue(viewModel.CanRun());
        }
        [TestMethod]
        public void CanRunTest4() {
            inputDataService.InputType = InputType.Unknown;
            viewModel.Path = @"some path";
            Assert.IsFalse(viewModel.CanRun());
        }
    }
}
#endif