#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileArchiver.Base;
using FileArchiver.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {

    #region MainView

    public sealed class TestMainWindow : MainWindow {
        public TestMainWindow() {
        }
        public MainViewModel ViewModel => (MainViewModel)DataContext;
    }

    #endregion

    [TestClass]
    public sealed class ViewTests {
        TestMainWindow window;
        MainViewModel viewModel;

        [TestInitialize]
        public void OnInitialize() {
            this.window = new TestMainWindow();
            this.window.Show();
            this.viewModel = window.ViewModel;
        }
        [TestCleanup]
        public void OnCleanup() {
            this.window.Close();
            this.window = null;
            this.viewModel = null;
        }
        [TestMethod]
        public void DefaultsTest() {
            Assert.IsTrue(window.ChoiceButton.IsEnabled);
            Assert.IsFalse(window.PathTextBlock.IsEnabled);
            Assert.IsFalse(window.RunButton.IsEnabled);
            Assert.IsFalse(window.ProgressBar.IsEnabled);
            Assert.IsFalse(window.StatusTextBlock.IsEnabled);

            Assert.AreEqual("(path)", window.PathTextBlock.Text);
            Assert.AreEqual("(status)", window.StatusTextBlock.Text);
            AreEqual(0, window.ProgressBar.Value);
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
            AreEqual(50, window.ProgressBar.Value);
            viewModel.ProgressValue = 100;
            AreEqual(100, window.ProgressBar.Value);
        }

        private void AreEqual(double expected, double actual) {
            if(MathHelper.AreNotEqual(expected, actual))
                Assert.Fail();
        }
    }
}
#endif