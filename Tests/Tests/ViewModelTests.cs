using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileArchiver.Core.Base;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;
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
        public TestIFileSelectorService()
            : this(NoneTraceableObject.Instance) {
        }
        public TestIFileSelectorService(ITraceableObject traceableObject)
            : base(traceableObject) {
        }

        #region IFileSelectorService

        string IFileSelectorService.GetSaveFile(string defaultExtension) {
            TraceMember();
            return FilePath;
        }

        #endregion

        public string FilePath { get; set; }
    }

    public class TestIFolderSelectorService : TraceableObject, IFolderSelectorService {
        public TestIFolderSelectorService()
            : this(NoneTraceableObject.Instance) {
        }
        public TestIFolderSelectorService(ITraceableObject traceableObject)
            : base(traceableObject) {
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
        public TestIInputDataService() 
            : this(NoneTraceableObject.Instance) {
        }
        public TestIInputDataService(ITraceableObject traceableObject)
            : base(traceableObject) {
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
        public TestIHuffmanEncodingService()
            : this(NoneTraceableObject.Instance) {
        }
        public TestIHuffmanEncodingService(ITraceableObject traceableObject)
            : base(traceableObject) {
        }

        #region IHuffmanEncodingService

        Task<EncodingStatistics> IHuffmanEncodingService.EncodeAsync(string inputPath, string outputFile, CancellationToken cancellationToken, IProgress<CodingProgressInfo> progress) {
            TraceMember();
            EncodeAction?.Invoke();
            return Task.FromResult(new EncodingStatistics(InputSize, OutputSize));
        }

        #endregion

        public long InputSize { get; set; } = 0;
        public long OutputSize { get; set; } = 0;
        public Action EncodeAction { get; set; }
    }

    public class TestIHuffmanDecodingService : TraceableObject, IHuffmanDecodingService {
        public TestIHuffmanDecodingService()
            : this(NoneTraceableObject.Instance) {
        }
        public TestIHuffmanDecodingService(ITraceableObject traceableObject)
            : base(traceableObject) {
        }

        #region IHuffmanDecodingService

        Task IHuffmanDecodingService.DecodeAsync(string inputFile, string outputFolder, CancellationToken cancellationToken, IProgress<CodingProgressInfo> progress) {
            TraceMember();
            DecodeAction?.Invoke();
            return Task.FromResult<object>(null);
        }

        #endregion

        public Action DecodeAction { get; set; }
    }

    #endregion

    [TestFixture]
    public class ViewModelTests {
        TestViewModel viewModel;
        TestIInputDataService inputDataService;
        TestIFileSelectorService fileSelectorService;
        TestIFolderSelectorService folderSelectorService;
        TestIHuffmanEncodingService encodingService;
        TestIHuffmanDecodingService decodingService;

        [SetUp]
        public void SetUp() {
            this.viewModel = new TestViewModel();
            this.inputDataService = (TestIInputDataService)viewModel.InputDataService;
            this.fileSelectorService = (TestIFileSelectorService)viewModel.FileSelectorService;
            this.folderSelectorService = (TestIFolderSelectorService)viewModel.FolderSelectorService;
            encodingService = (TestIHuffmanEncodingService)viewModel.EncodingService;
            decodingService = (TestIHuffmanDecodingService)viewModel.DecodingService;
        }
        [TearDown]
        public void TearDown() {
            this.viewModel = null;
            this.inputDataService = null;
        }
        [Test]
        public void DefaultsTest() {
            Assert.IsFalse(viewModel.CanRun());
            Assert.AreEqual(ViewModelStatus.WaitForCommand, viewModel.Status);
        }
        [Test]
        public async Task EncodeFileTest1() {
            inputDataService.InputCommand = InputCommand.Encode;
            viewModel.Path = "Path";

            fileSelectorService.FilePath = @"C:\File.archive";
            await viewModel.Run();
            Assert.AreEqual("Run;GetInputCommand;GetSaveFile;EncodeAsync;", viewModel.Trace);
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
            Assert.AreEqual("Run;GetInputCommand;GetSaveFile;EncodeAsync;", viewModel.Trace);
        }
        [Test]
        public async Task DecodeFileTest1() {
            inputDataService.InputCommand = InputCommand.Decode;
            viewModel.Path = "Path";

            folderSelectorService.FolderPath = @"C:\Folder";
            await viewModel.Run();
            Assert.AreEqual("Run;GetInputCommand;GetFolder;DecodeAsync;", viewModel.Trace);
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

            Assert.ThrowsAsync<Exception>(async () => await viewModel.Run());
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
        [Test]
        public async Task StatusPropertyTest1() {
            encodingService.EncodeAction = () => {
                Assert.AreEqual(ViewModelStatus.Encoding, viewModel.Status);
            };
            Assert.AreEqual(ViewModelStatus.WaitForCommand, viewModel.Status);

            inputDataService.InputCommand = InputCommand.Encode;
            fileSelectorService.FilePath = @"C:\File.archive";
            await viewModel.Run();
            Assert.AreEqual(ViewModelStatus.WaitForCommand | ViewModelStatus.EncodingFinished, viewModel.Status);
            await viewModel.Run();
            Assert.AreEqual(ViewModelStatus.WaitForCommand | ViewModelStatus.EncodingFinished, viewModel.Status);
        }
        [Test]
        public async Task StatusPropertyTest2() {
            decodingService.DecodeAction = () => {
                Assert.AreEqual(ViewModelStatus.Decoding, viewModel.Status);
            };
            Assert.AreEqual(ViewModelStatus.WaitForCommand, viewModel.Status);

            inputDataService.InputCommand = InputCommand.Decode;
            folderSelectorService.FolderPath = @"C:\Folder";
            await viewModel.Run();
            Assert.AreEqual(ViewModelStatus.WaitForCommand | ViewModelStatus.DecodingFinished, viewModel.Status);
            await viewModel.Run();
            Assert.AreEqual(ViewModelStatus.WaitForCommand | ViewModelStatus.DecodingFinished, viewModel.Status);
        }
        [Test]
        public async Task StatusPropertyTest3() {
            encodingService.EncodeAction = () => throw new Exception();
            Assert.AreEqual(ViewModelStatus.WaitForCommand, viewModel.Status);

            inputDataService.InputCommand = InputCommand.Encode;
            fileSelectorService.FilePath = @"C:\File.archive";
            try { await viewModel.Run(); }
            catch { /*ignored*/ }
            Assert.AreEqual(ViewModelStatus.Error, viewModel.Status);
        }
        [Test]
        public async Task StatusPropertyTest4() {
            decodingService.DecodeAction = () => throw new Exception();
            Assert.AreEqual(ViewModelStatus.WaitForCommand, viewModel.Status);

            inputDataService.InputCommand = InputCommand.Decode;
            folderSelectorService.FolderPath = @"C:\Folder";
            try { await viewModel.Run(); }
            catch { /*ignored*/ }
            Assert.AreEqual(ViewModelStatus.Error, viewModel.Status);
        }
        [Test]
        public async Task StatusPropertyTest5() {
            encodingService.EncodeAction = () => throw new OperationCanceledException();
            Assert.AreEqual(ViewModelStatus.WaitForCommand, viewModel.Status);

            inputDataService.InputCommand = InputCommand.Encode;
            fileSelectorService.FilePath = @"C:\File.archive";
            await viewModel.Run();
            Assert.AreEqual(ViewModelStatus.Cancelled, viewModel.Status);
        }
        [Test]
        public async Task StatusPropertyTest6() {
            decodingService.DecodeAction = () => throw new OperationCanceledException();
            Assert.AreEqual(ViewModelStatus.WaitForCommand, viewModel.Status);

            inputDataService.InputCommand = InputCommand.Decode;
            folderSelectorService.FolderPath = @"C:\Folder";
            await viewModel.Run();
            Assert.AreEqual(ViewModelStatus.Cancelled, viewModel.Status);
        }
    }
}
