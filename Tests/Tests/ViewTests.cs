using System;
using FileArchiver.Core;
using FileArchiver.IOC;
using FileArchiver.Core.ViewModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using FileArchiver.Core.Controls;
using FileArchiver.Core.Helpers;
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
