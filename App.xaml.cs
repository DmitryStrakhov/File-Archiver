using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FileArchiver.Services;
using FileArchiver.ViewModel;

namespace FileArchiver {
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = new MainViewModel(new DefaultServiceFactory());
            mainWindow.Show();
        }
    }
}