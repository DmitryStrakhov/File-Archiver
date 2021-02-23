using System;
using System.Globalization;
using System.Windows;
using FileArchiver.Core.ViewModel;

namespace FileArchiver.Core.Converters {
    public class CancelLinkVisibilityConverter : SimpleValueConverterBase<ViewModelStatus, Visibility> {
        protected override Visibility ConvertCore(ViewModelStatus value) {
            return (value & (ViewModelStatus.Encoding | ViewModelStatus.Decoding)) != 0
                ? Visibility.Visible
                : Visibility.Hidden;
        }
    }
}