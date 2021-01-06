using System;
using System.Windows;
using FileArchiver.Core;
using FileArchiver.IOC;

namespace FileArchiver.Runner {
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            Ioc ioc = new Ioc();
            MainWindow mainWindow = ioc.Resolve<MainWindow>();
            mainWindow.Show();
        }
    }
}