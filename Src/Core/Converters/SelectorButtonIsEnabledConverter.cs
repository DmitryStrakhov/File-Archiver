using System;
using System.Globalization;
using System.Windows.Controls;
using FileArchiver.Core.ViewModel;

namespace FileArchiver.Core.Converters {
    public class SelectorButtonIsEnabledConverter : SimpleValueConverterBase<ViewModelStatus, bool> {
        protected override bool ConvertCore(ViewModelStatus value) {
            if(value.IsEncodingOrDecodingPerforming()) return false;
            return true;
        }
    }
}