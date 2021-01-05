using System.Collections;
using System.Collections.Generic;

namespace FileArchiver.DataStructures {
    public abstract class EnumerableBase<T> : IEnumerable<T> {
        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() {
            return CreateEnumerator();
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return CreateEnumerator();
        }

        #endregion

        protected abstract IEnumerator<T> CreateEnumerator();
    }
}