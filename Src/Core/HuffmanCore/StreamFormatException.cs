using System;

namespace FileArchiver.Core.HuffmanCore {
    public class StreamFormatException : Exception {
        public StreamFormatException(string message = null)
            : base(message ?? DefaultMessage) {
        }
        const string DefaultMessage = "Input stream has incorrect format.";
    }
}