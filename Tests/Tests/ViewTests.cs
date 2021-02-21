using System;
using System.IO;
using System.Linq;
using FileArchiver.Core;
using FileArchiver.IOC;
using FileArchiver.Core.ViewModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using FileArchiver.Core.Base;
using FileArchiver.Core.Builders;
using FileArchiver.Core.Controls;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.Parsers;
using FileArchiver.Core.Services;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture, Apartment(ApartmentState.STA)]
    public sealed class ViewTests {
        MainWindow window;
        MainViewModel viewModel;
        Ioc ioc;

        [OneTimeSetUp]
        public void OneTimeSetUp() {
            ioc = new Ioc();
        }
        [OneTimeTearDown]
        public void OneTimeTearDown() {
            ioc.Dispose();
        }
        [SetUp]
        public void SetUp() {
            this.window = ioc.Resolve<MainWindow>();
            this.window.ShowInTaskbar = false;
            this.viewModel = (MainViewModel)window.DataContext;
            this.window.Show();
        }
        [TearDown]
        public void TearDown() {
            this.window.Close();
            this.window = null;
            this.viewModel = null;
        }
        [Test, Explicit]
        public void DisplayTest() {
            while(window.IsVisible) {
                window.Dispatcher.Invoke(DispatcherPriority.Background, (Action)(() => { }));
            }
        }
        [Test, Explicit]
        public void DisplayValidationErrorsTest() {
            viewModel.Path = "some path";

            while(window.IsVisible) {
                window.Dispatcher.Invoke(DispatcherPriority.Background, (Action)(() => { }));
            }
        }
        [Test]
        public void DefaultsTest() {
            Assert.IsTrue(window.ChoiceButton.IsEnabled);
            Assert.IsFalse(window.RunButton.IsEnabled);

            Assert.AreEqual(string.Empty, window.PathTextBlock.Text);
            Assert.AreEqual("[status]", window.StatusTextBlock.Text);
            AreEqual(0, window.ProgressBar.Value);
            Assert.IsFalse(window.CancelLink.IsVisible);

            CollectionAssert.IsEmpty(Validation.GetErrors(window.PathTextBlock));
        }
        [Test]
        public void PathPropertyTest() {
            viewModel.Path = "value1";
            Assert.AreEqual("value1", window.PathTextBlock.Text);
            viewModel.Path = "value2";
            Assert.AreEqual("value2", window.PathTextBlock.Text);
        }
        [Test]
        public void StatusMessagePropertyTest() {
            viewModel.StatusMessage = "value3";
            Assert.AreEqual("value3", window.StatusTextBlock.Text);
            viewModel.StatusMessage = "value4";
            Assert.AreEqual("value4", window.StatusTextBlock.Text);
        }
        [Test]
        public void ProgressValuePropertyTest1() {
            viewModel.ProgressValue = 50;
            AreEqual(50, window.ProgressBar.Value);
            AreEqual(50, window.ProgressBar.ProgressValue);
            viewModel.ProgressValue = 100;
            AreEqual(100, window.ProgressBar.Value);
            AreEqual(100, window.ProgressBar.ProgressValue);
        }
        [Test]
        public void ProgressValuePropertyTest2() {
            ProgressBarControl progressBar = window.ProgressBar;
            progressBar.IndeterminateValue = -1;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;

            progressBar.ProgressValue = 1;
            AssertProgressBar(1, 1, false);
            progressBar.ProgressValue = -10;
            AssertProgressBar(0, 0, false);
            progressBar.ProgressValue = 101;
            AssertProgressBar(100, 100, false);

            progressBar.ProgressValue = -1;
            AssertProgressBar(100, -1, true);
            progressBar.ProgressValue = 30;
            AssertProgressBar(30, 30, false);
        }
        [Test]
        public void PathValidationErrorTest() {
            viewModel.Path = "Some Path";
            var errors = Validation.GetErrors(window.PathTextBlock);
            Assert.AreEqual(1, errors.Count);
        }
        [Test]
        public void CancelLinkTest() {
            ControlAssert.PropertyIsBound(window.CancelLink, () => UIElement.VisibilityProperty, nameof(viewModel.Status));
            Hyperlink hyperlink = (Hyperlink)window.CancelLink.Inlines.Single();
            ControlAssert.PropertyIsBound(hyperlink, () => Hyperlink.CommandProperty, nameof(viewModel.CancelCommand));
        }
        [Test]
        public async Task CancelLinkVisibilityTest1() {
            TestIFileSelectorService fileSelector = new TestIFileSelectorService();
            fileSelector.FilePath = @"path";
            viewModel.FileSelectorService = fileSelector;

            TestIInputDataService inputDataService = new TestIInputDataService();
            inputDataService.InputCommand = InputCommand.Encode;
            viewModel.InputDataService = inputDataService;
            
            TestIHuffmanEncodingService encodingService = new TestIHuffmanEncodingService();
            encodingService.EncodeAction = () => {
                Assert.AreEqual(Visibility.Visible, window.CancelLink.Visibility);
            };
            viewModel.EncodingService = encodingService;

            Assert.AreEqual(Visibility.Hidden, window.CancelLink.Visibility);
            await viewModel.Run();
            Assert.AreEqual(Visibility.Hidden, window.CancelLink.Visibility);
        }
        [Test]
        public async Task CancelLinkVisibilityTest2() {
            TestIFolderSelectorService folderSelector = new TestIFolderSelectorService();
            folderSelector.FolderPath = "path";
            viewModel.FolderSelectorService = folderSelector;

            TestIInputDataService inputDataService = new TestIInputDataService();
            inputDataService.InputCommand = InputCommand.Decode;
            viewModel.InputDataService = inputDataService;

            TestIHuffmanDecodingService decodingService = new TestIHuffmanDecodingService();
            decodingService.DecodeAction = () => {
                Assert.AreEqual(Visibility.Visible, window.CancelLink.Visibility);
            };
            viewModel.DecodingService = decodingService;

            Assert.AreEqual(Visibility.Hidden, window.CancelLink.Visibility);
            await viewModel.Run();
            Assert.AreEqual(Visibility.Hidden, window.CancelLink.Visibility);
        }
        [Test]
        public async Task EncodingCancellationTest1() {
            TestIFileSelectorService fileSelector = new TestIFileSelectorService();
            fileSelector.FilePath = @"path";
            viewModel.FileSelectorService = fileSelector;
            viewModel.Path = @"C:\dir\";

            TestIInputDataService inputDataService = new TestIInputDataService();
            inputDataService.InputCommand = InputCommand.Encode;
            viewModel.InputDataService = inputDataService;

            TestIFileSystemService fileSystem = new TestIFileSystemService();
            fileSystem.EnumFileSystemEntriesFunc = _ => {
                return new[] {
                    new FileSystemEntry(FileSystemEntryType.Directory, "dir", @"C:\dir\"),
                    new FileSystemEntry(FileSystemEntryType.File, "f1.dat", @"C:\dir\f1.dat"),
                    new FileSystemEntry(FileSystemEntryType.File, "f2.dat", @"C:\dir\f2.dat"),
                };
            };

            WritableMemoryStream outputStream = new WritableMemoryStream();
            TestIPlatformService platform = new TestIPlatformService();
            platform.WriteFileFunc = _ => outputStream;

            int fileRequest = 0;
            platform.ReadFileFunc = x => {
                if(++fileRequest == 4) viewModel.Cancel();
                switch(x) {
                    case @"C:\dir\f1.dat": return new MemoryStream(new byte[200 * 1024]);
                    case @"C:\dir\f2.dat": return new MemoryStream(new byte[300 * 1024]);
                    default: throw new NotImplementedException();
                }
            };
            viewModel.EncodingService = new DefaultHuffmanEncodingService(fileSystem, platform, new DefaultStreamBuilder());

            await viewModel.Run();
            Assert.AreEqual(32855, outputStream.Length);
        }
        [Test]
        public async Task EncodingCancellationTest2() {
            TestIFolderSelectorService folderSelector = new TestIFolderSelectorService();
            folderSelector.FolderPath = @"C:\folder\";
            viewModel.FolderSelectorService = folderSelector;
            viewModel.Path = @"C:\dir\";

            TestIInputDataService inputDataService = new TestIInputDataService();
            inputDataService.InputCommand = InputCommand.Decode;
            viewModel.InputDataService = inputDataService;

            WritableMemoryStream outputStream = new WritableMemoryStream();
            TestIPlatformService platform = new TestIPlatformService();
            platform.WriteFileFunc = x => {
                if(string.Equals(x, @"C:\folder\dir\f2.dat", StringComparison.OrdinalIgnoreCase)) viewModel.Cancel();
                return outputStream;
            };
            platform.ReadFileFunc = x => new BufferBuilder()
                .AddByte(0x2)
                .AddInt(0x9)
                .AddByte(0x0)
                .AddLong(0x1)
                .AddByte(0x1)
                .AddInt(0x6)
                .AddString("dir")
                .AddInt(2)
                .AddByte(0x0)
                .AddInt(0xC)
                .AddString("f1.dat")
                .AddLong(200 * 1024 * 8) // 200Kb
                .AddByte(0x0, 200 * 1024)
                .AddByte(0x0)
                .AddInt(0xC)
                .AddString("f2.dat")
                .AddLong(300 * 1024 * 8) // 300Kb
                .AddByte(0x0, 300 * 1024).GetStream();

            viewModel.DecodingService = new DefaultHuffmanDecodingService(platform, new DefaultStreamParser());
            await viewModel.Run();
            Assert.AreEqual(256 * 1024 * 8, outputStream.Length);
        }

        private void AssertProgressBar(double value, double progressValue, bool isIndeterminate) {
            ProgressBarControl progressBar = window.ProgressBar;
            AreEqual(value, progressBar.Value);
            AreEqual(progressValue, progressBar.ProgressValue);
            Assert.AreEqual(isIndeterminate, progressBar.IsIndeterminate);
        }
        private void AreEqual(double expected, double actual) {
            if(MathHelper.AreNotEqual(expected, actual)) Assert.Fail();
        }
    }
}
