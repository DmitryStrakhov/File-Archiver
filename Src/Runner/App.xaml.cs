using System;
using System.Windows;
using System.Windows.Threading;
using FileArchiver.Core;
using FileArchiver.Core.View;
using FileArchiver.IOC;

namespace FileArchiver.Runner {
    public partial class App : Application {
        public App() {
            DispatcherUnhandledException += DispatchUnhandledException;
        }
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            using(Ioc ioc = new Ioc()) {
                ioc.Resolve<MainWindow>().Show();
            }
        }
        private void DispatchUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            e.Handled = !IsCriticalException(e.Exception);

            UnhandledExceptionWindow dialog = new UnhandledExceptionWindow(e.Exception);
            dialog.Owner = MainWindow;
            dialog.ShowDialog();
        }
        private bool IsCriticalException(Exception e) {
            return e is StackOverflowException
                   || e is OutOfMemoryException
                   || e is NullReferenceException;
        }
    }
}