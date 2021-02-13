using System.Collections.Generic;
using FileArchiver.Core.HuffmanCore;

namespace FileArchiver.Tests {
    public class TestIProgressHandler : IProgressHandler {
        readonly List<long> valueList;
        IProgressState state;

        public TestIProgressHandler() {
            this.valueList = new List<long>(1024);
            this.state = null;
        }

        #region IProgressHandler

        IProgressState IProgressHandler.State {
            get { return state; }
            set { state = value; }
        }
        void IProgressHandler.Report(long byteCount, string statusMessage) {
            valueList.Add(byteCount);
        }

        #endregion

        public IReadOnlyCollection<long> ValueList { get { return valueList; } }
    }
}