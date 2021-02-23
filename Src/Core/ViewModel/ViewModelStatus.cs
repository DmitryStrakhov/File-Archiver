using System;

namespace FileArchiver.Core.ViewModel {
    [Flags]
    public enum ViewModelStatus {
        WaitForCommand = 1,
        Encoding = 2,
        Decoding = 4,
        EncodingFinished = 8,
        DecodingFinished = 16,
        Error = 32,
        Cancelled = 64,
    }

    public static class ViewModelStatusExtensions {
        public static bool IsEncodingOrDecodingPerforming(this ViewModelStatus @this) {
            return (@this & (ViewModelStatus.Encoding | ViewModelStatus.Decoding)) != 0;
        }
    }
}