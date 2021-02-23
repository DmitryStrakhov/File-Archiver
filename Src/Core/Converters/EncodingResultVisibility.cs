using System;
using System.Globalization;
using System.Windows;
using FileArchiver.Core.ViewModel;

namespace FileArchiver.Core.Converters {
    public class EncodingResultVisibilityConverter : SimpleValueConverterBase<ViewModelStatus, Visibility> {
        protected override Visibility ConvertCore(ViewModelStatus value) {
            return value == (ViewModelStatus.EncodingFinished | ViewModelStatus.WaitForCommand)
                ? Visibility.Visible
                : Visibility.Hidden;
        }
    }
}