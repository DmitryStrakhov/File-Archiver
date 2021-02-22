using System;
using System.Globalization;
using FileArchiver.Core.FormatProviders;
using FileArchiver.Core.Helpers;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.ViewModel {
    public class EncodingResultViewModel {
        readonly Lazy<string> inputSizeTexLazy;
        readonly Lazy<string> outputSizeTexLazy;
        readonly Lazy<string> saveFactorTexLazy;
        readonly EncodingStatistics statistics;

        public EncodingResultViewModel(EncodingStatistics encodingStatistics) {
            Guard.IsNotNull(encodingStatistics, nameof(encodingStatistics));
            this.statistics = encodingStatistics;

            inputSizeTexLazy = new Lazy<string>(() =>
                string.Format(FileSizeFormatProvider.Instance, "Input Size: {0:FS}", statistics.InputSize));

            outputSizeTexLazy = new Lazy<string>(() =>
                string.Format(FileSizeFormatProvider.Instance, "Output Size: {0:FS}", statistics.OutputSize));

            saveFactorTexLazy = new Lazy<string>(() => {
                if(statistics.OutputSize == 0) return "(infinity)";
                double value = (1 - (double)statistics.OutputSize / statistics.InputSize) * 100;
                return $"Save Factor: {value.ToString("F02", CultureInfo.InvariantCulture)}%";
            });
        }
        public string InputSize {
            get { return inputSizeTexLazy.Value; }
        }
        public string OutputSize {
            get { return outputSizeTexLazy.Value; }
        }
        public string SaveFactor {
            get { return saveFactorTexLazy.Value; }
        }
    }
}