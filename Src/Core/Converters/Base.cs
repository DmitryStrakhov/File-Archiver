using System;
using System.Globalization;
using System.Windows.Data;

namespace FileArchiver.Core.Converters {
    public abstract class SimpleValueConverterBase<TI, TR> : IValueConverter {
        public object Convert(object value, Type targetType, object param, CultureInfo culture) {
            return ConvertCore((TI)value);
        }
        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture) {
            return ConvertBackCore((TR)value);
        }
        protected abstract TR ConvertCore(TI value);
        protected virtual TI ConvertBackCore(TR value) => throw new NotImplementedException();
    }
}