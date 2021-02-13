using System;

namespace FileArchiver.Core.HuffmanCore {
    public class CodingProgressState : IProgressState {
        public long Value { get; set; }

        public CodingProgressState WithValue(long value) {
            Value = value;
            return this;
        }
    }
}