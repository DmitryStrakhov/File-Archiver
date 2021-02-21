using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FileArchiver.Core.ViewModel;

namespace FileArchiver.Core.Converters {
    public class CancelLinkVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object param, CultureInfo culture) {
            ViewModelStatus stats = (ViewModelStatus)value;
            return stats != ViewModelStatus.WaitForCommand ? Visibility.Visible : Visibility.Hidden;
        }
        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}