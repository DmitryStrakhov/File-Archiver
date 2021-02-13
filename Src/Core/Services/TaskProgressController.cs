using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Core.Services {
    public interface ITaskProgressController : IProgressHandler {
        void Start();
        void Finish();
        void StartIndeterminate();
        void EndIndeterminate();
    }

    public class NullTaskProgressController : ITaskProgressController {
        public void Start() {
        }
        public void Finish() {
        }
        public void Report(long byteCount, string statusMessage) {
        }
        public void StartIndeterminate() {
        }
        public void EndIndeterminate() {
        }
        IProgressState IProgressHandler.State { get { return null; } set { } }
    }
}