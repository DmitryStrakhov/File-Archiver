using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FileArchiver.Core.ViewModel;

namespace FileArchiver.Core.Converters {
    public class EncodingResultVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            ViewModelStatus status = (ViewModelStatus)value;
            return status == (ViewModelStatus.EncodingFinished | ViewModelStatus.WaitForCommand) ? Visibility.Visible : Visibility.Hidden;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}