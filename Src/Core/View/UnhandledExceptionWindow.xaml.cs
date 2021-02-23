using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.ViewModel;

namespace FileArchiver.Core.View {
    public partial class UnhandledExceptionWindow : Window {
        public UnhandledExceptionWindow(Exception exception) {
            Guard.IsNotNull(exception, nameof(exception));

            DataContext = new UnhandledExceptionViewModel(exception);
            InitializeComponent();
        }
    }
}
