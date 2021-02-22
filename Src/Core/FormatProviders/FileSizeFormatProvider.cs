using System;
using System.Globalization;

namespace FileArchiver.Core.FormatProviders {
    public class FileSizeFormatProvider : IFormatProvider, ICustomFormatter {
        public static readonly FileSizeFormatProvider Instance = new FileSizeFormatProvider();

        private FileSizeFormatProvider() {
        }

        public object GetFormat(Type formatType) {
            if(formatType == typeof(ICustomFormatter)) {
                return this;
            }
            return null;
        }
        public string Format(string format, object arg, IFormatProvider formatProvider) {
            if(arg.GetType() != typeof(long) || !string.Equals(format, "FS", StringComparison.OrdinalIgnoreCase)) {
                try {
                    return HandleOtherFormats(format, arg);
                }
                catch(FormatException e) {
                    throw new FormatException($"The format of '{format}' is invalid.", e);
                }
            }
            long value = (long)arg;

            if(value < 1000) {
                return FormatResult(value, 1, "Bytes");
            }
            if(value < 1000000) {
                return FormatResult(value, 1000, "Kb");
            }
            if(value < 1000000000) {
                return FormatResult(value, 1000000, "Mb");
            }
            return FormatResult(value, 1000000000, "Gb");
        }
        private string FormatResult(long size, int divider, string suffix) {
            double value = Math.Round((double)size / divider, 2);
            return value.ToString(CultureInfo.InvariantCulture) + " " + suffix;
        }
        
        private string HandleOtherFormats(string format, object arg) {
            if(arg is IFormattable formattable) {
                return formattable.ToString(format, CultureInfo.CurrentCulture);
            }
            return arg != null ? arg.ToString() : string.Empty;
        }
    }
}