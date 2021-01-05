using System;
using System.Diagnostics;
using FileArchiver.Helpers;

namespace FileArchiver.DataStructures {
    [DebuggerDisplay("ValueCache()")]
    public sealed class ValueCache<T> {
        readonly Func<T> getValue;
        ValueBox valueBox;

        public ValueCache(Func<T> getValue) {
            Guard.IsNotNull(getValue, nameof(getValue));
            this.getValue = getValue;
            this.valueBox = null;
        }
        public void Reset() {
            valueBox = null;
        }
        public T Value {
            get {
                if(valueBox != null) {
                    return valueBox.Value;
                }
                return LazyInitValue();
            }
        }
        private T LazyInitValue() {
            return (valueBox = new ValueBox(getValue())).Value;
        }

        #region ValueBox

        sealed class ValueBox {
            public readonly T Value;

            internal ValueBox(T value) {
                this.Value = value;
            }
        }

        #endregion
    }
}