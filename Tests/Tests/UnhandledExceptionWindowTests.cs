using System;
using System.Threading;
using FileArchiver.Core.View;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace FileArchiver.Tests {
    [TestFixture, Apartment(ApartmentState.STA)]
    public class UnhandledExceptionWindowTests {
        [Test, Explicit]
        public void ShowUpTest() {
            Exception exception = Exception<Exception>();
            UnhandledExceptionWindow window = CreateWindow(exception);
            try {
                window.Show();
                window.DoDispatcherLoop();
            }
            finally {
                window.Close();
            }
        }
        [Test]
        public void Test1() {
            InvalidOperationException e = Exception<InvalidOperationException>();
            UnhandledExceptionWindow window = CreateWindow(e);

            try {
                window.Show();
                Assert.AreEqual("InvalidOperationException", window.ExceptionNameTextBlock.Text);
                AssertWithPattern(@"^Message:.*Call stack.*$", window.ExceptionDetailsTextBlock.Text);
            }
            finally {
                window.Close();
            }
        }
        [Test]
        public void Test2() {
            Exception e = ExceptionWithInner<InvalidOperationException>();
            UnhandledExceptionWindow window = CreateWindow(e);

            try {
                window.Show();
                Assert.AreEqual("Exception", window.ExceptionNameTextBlock.Text);
                AssertWithPattern(@"^Message:.*Call stack.*Inner Exception: InvalidOperationException.*Message:.*Call stack:.*", window.ExceptionDetailsTextBlock.Text);
            }
            finally {
                window.Close();
            }
        }
        private void AssertWithPattern(string pattern, string input) {
            Regex regex = new Regex(pattern, RegexOptions.Singleline);
            if(regex.IsMatch(input)) return;
            throw new AssertionException($"'{input}' doesn't match to '{pattern}' pattern");
        }
        
        private UnhandledExceptionWindow CreateWindow(Exception e) {
            return new UnhandledExceptionWindow(e) {
                ShowInTaskbar = false
            };
        }
        private T Exception<T>() where T : Exception, new() {
            try {
                throw new T();
            }
            catch(T e) {
                return e;
            }
        }
        private Exception ExceptionWithInner<TInnerException>() where TInnerException : Exception, new() {
            try {
                Exception innerException = Exception<TInnerException>();
                throw new Exception("message", innerException);
            }
            catch(Exception e) {
                return e;
            }
        }
    }
}