#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using FileArchiver.Services;
using FileArchiver.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {

    #region MainView

    public sealed class TestMainWindow : MainWindow {
        public TestMainWindow() {
            DataContext = new MainViewModel(new DefaultServiceFactory());
        }
        public MainViewModel ViewModel {
            get { return (MainViewModel)DataContext; }
        }
    }

    #endregion

    [TestClass]
    public sealed class ViewTests {
        TestMainWindow window;
        MainViewModel viewModel;

        [TestInitialize]
        public void OnInitialize() {
            this.window = new TestMainWindow();
            this.viewModel = window.ViewModel;
            this.window.Show();
        }
        [TestCleanup]
        public void OnCleanup() {
            this.window.Close();
            this.window = null;
            this.viewModel = null;
        }
        [TestMethod, Ignore]
        public void DisplayTest() {
            while(window.IsVisible) {
                Action action = () => { };
                window.Dispatcher.Invoke(DispatcherPriority.Background, action);
            }
        }
        [TestMethod, Ignore]
        public void DisplayValidationErrorsTest() {
            viewModel.Path = "some path";

            while(window.IsVisible) {
                Action action = () => { };
                window.Dispatcher.Invoke(DispatcherPriority.Background, action);
            }
        }
        [TestMethod]
        public void DefaultsTest() {
            Assert.IsTrue(window.ChoiceButton.IsEnabled);
            Assert.IsFalse(window.RunButton.IsEnabled);
            Assert.IsFalse(window.ProgressBar.IsEnabled);
            Assert.IsFalse(window.StatusTextBlock.IsEnabled);

            Assert.AreEqual(string.Empty, window.PathTextBlock.Text);
            Assert.AreEqual(string.Empty, window.StatusTextBlock.Text);
            AssertHelper.AreEqual(0, window.ProgressBar.Value);
            
            AssertHelper.CollectionIsEmpty(Validation.GetErrors(window.PathTextBlock));
        }
        [TestMethod]
        public void PathPropertyTest() {
            viewModel.Path = "value1";
            Assert.AreEqual("value1", window.PathTextBlock.Text);
            viewModel.Path = "value2";
            Assert.AreEqual("value2", window.PathTextBlock.Text);
        }
        [TestMethod]
        public void StatusPropertyTest() {
            viewModel.Status = "value3";
            Assert.AreEqual("value3", window.StatusTextBlock.Text);
            viewModel.Status = "value4";
            Assert.AreEqual("value4", window.StatusTextBlock.Text);
        }
        [TestMethod]
        public void ProgressValuePropertyTest() {
            viewModel.ProgressValue = 50;
            AssertHelper.AreEqual(50, window.ProgressBar.Value);
            viewModel.ProgressValue = 100;
            AssertHelper.AreEqual(100, window.ProgressBar.Value);
        }
        [TestMethod]
        public void PathValidationErrorTest() {
            viewModel.Path = "Some Path";
            var errors = Validation.GetErrors(window.PathTextBlock);
            Assert.AreEqual(1, errors.Count);
        }
    }
}
#endif