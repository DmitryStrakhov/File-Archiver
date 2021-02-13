namespace FileArchiver.Core.HuffmanCore {
    public interface IProgressHandler {
        void Report(long byteCount, string statusMessage);
        IProgressState State { get; set; }
    }

    public interface IProgressState {
    }
}