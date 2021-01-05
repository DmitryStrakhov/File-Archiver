﻿using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using FileArchiver.Core;
using FileArchiver.Core.Services;
using FileArchiver.Core.ViewModel;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture, Apartment(ApartmentState.STA)]
    public sealed class ViewTests {
        MainWindow window;
        MainViewModel viewModel;

        [SetUp]
        public void OnInitialize() {
            this.window = new MainWindow {ShowInTaskbar = false};
            this.viewModel = new MainViewModel(new DefaultServiceFactory());
            this.window.DataContext = viewModel;
            this.window.Show();
        }
        [TearDown]
        public void OnCleanup() {
            this.window.Close();
            this.window = null;
            this.viewModel = null;
        }
        [Test, Explicit]
        public void DisplayTest() {
            while(window.IsVisible) {
                Action action = () => { };
                window.Dispatcher.Invoke(DispatcherPriority.Background, action);
            }
        }
        [Test, Explicit]
        public void DisplayValidationErrorsTest() {
            viewModel.Path = "some path";

            while(window.IsVisible) {
                Action action = () => { };
                window.Dispatcher.Invoke(DispatcherPriority.Background, action);
            }
        }
        [Test]
        public void DefaultsTest() {
            Assert.IsTrue(window.ChoiceButton.IsEnabled);
            Assert.IsFalse(window.RunButton.IsEnabled);
            Assert.IsFalse(window.ProgressBar.IsEnabled);
            Assert.IsFalse(window.StatusTextBlock.IsEnabled);

            Assert.AreEqual(string.Empty, window.PathTextBlock.Text);
            Assert.AreEqual(string.Empty, window.StatusTextBlock.Text);
            AssertHelper.AreEqual(0, window.ProgressBar.Value);
            
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
        public void StatusPropertyTest() {
            viewModel.Status = "value3";
            Assert.AreEqual("value3", window.StatusTextBlock.Text);
            viewModel.Status = "value4";
            Assert.AreEqual("value4", window.StatusTextBlock.Text);
        }
        [Test]
        public void ProgressValuePropertyTest() {
            viewModel.ProgressValue = 50;
            AssertHelper.AreEqual(50, window.ProgressBar.Value);
            viewModel.ProgressValue = 100;
            AssertHelper.AreEqual(100, window.ProgressBar.Value);
        }
        [Test]
        public void PathValidationErrorTest() {
            viewModel.Path = "Some Path";
            var errors = Validation.GetErrors(window.PathTextBlock);
            Assert.AreEqual(1, errors.Count);
        }
    }
}
