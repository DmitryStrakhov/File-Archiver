using System;

namespace FileArchiver.Core.ViewModel {
    [Flags]
    public enum ViewModelStatus {
        WaitForCommand = 1,
        Encoding = 2,
        Decoding = 4,
        EncodingFinished = 8,
        DecodingFinished = 16,
    }
}