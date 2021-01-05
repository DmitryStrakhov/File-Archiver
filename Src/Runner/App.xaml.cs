using System;
using System.Windows;
using FileArchiver.Services;
using FileArchiver.ViewModel;

namespace FileArchiver.Runner {
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = new MainViewModel(new DefaultServiceFactory());
            mainWindow.Show();
        }
    }
}
