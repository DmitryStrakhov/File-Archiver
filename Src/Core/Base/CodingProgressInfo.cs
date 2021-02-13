using System;
using System.Diagnostics;

namespace FileArchiver.Core.Base {
    [DebuggerDisplay("CodingProgressInfo(Value={" + nameof(Value) + ("}, StatusMessage={" + nameof(StatusMessage) + "})"))]
    public struct CodingProgressInfo {
        public CodingProgressInfo(int value, string statusMessage) {
            Value = value;
            StatusMessage = statusMessage;
        }
        public int Value { get; }
        public string StatusMessage { get; }
    }
}