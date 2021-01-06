using System;
using System.Windows;
using FileArchiver.Core.ViewModel;

namespace FileArchiver.Core {
    public partial class MainWindow : Window {
        public MainWindow(MainViewModel viewModel) {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
