using System;
using System.Collections.Generic;
using System.Diagnostics;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.DataStructures {
    [DebuggerDisplay("BitSequence(Size={Size})")]
    public sealed class BitSequence : EnumerableBase<Bit> {
        byte[] data;
        int size;

        public BitSequence() : this(2) {
        }
        private BitSequence(int capacity) {
            this.size = 0;
            this.data = new byte[capacity];
        }
        public Bit this[int index] {
            get {
                Guard.IsInRange(index, 0, size - 1, nameof(index));
                int slot = data[index / 8];
                return ((slot >> (index % 8)) & 1) == 0 ? Bit.Zero : Bit.One;
            }
            set {
                Guard.IsInRange(index, 0, int.MaxValue, nameof(index));
                EnsureData(index);

                if(value == Bit.Zero)
                    data[index / 8] &= (byte)~(1 << index % 8);
                else
                    data[index / 8] |= (byte)(1 << index % 8);
                size = Math.Max(size, index + 1);
            }
        }
        public void Reduce(int index) {
            Guard.IsInRange(index, 0, int.MaxValue, nameof(index));
            if(index >= size - 1) return;

            for(int n = index + 1; n < size; n++) {
                this[n] = Bit.Zero;
            }
            size = index + 1;
        }
        public BitSequence Clone() {
            BitSequence clone = new BitSequence(data.Length);
            clone.size = size;
            Array.Copy(data, clone.data, clone.data.Length);
            return clone;
        }
        private void EnsureData(int index) {
            int capacity = index / 8 + 1;

            if(capacity > data.Length) {
                Array.Resize(ref data, capacity * 2);
            }
        }

        #region Equals & GetHashCode

        public override bool Equals(object obj) {
            BitSequence other = obj as BitSequence;
            if(other == null || size != other.size) return false;

            for(int n = 0; n < size; n++) {
                if(this[n] != other[n]) return false;
            }
            return true;
        }
        public override int GetHashCode() {
            return 0;
        }

        #endregion

        public int Size { get { return size; } }

        public static BitSequence FromString(string bitString) {
            Guard.IsNotNull(bitString, nameof(bitString));
            BitSequence sequence = new BitSequence(bitString.Length / 8 + 1);

            for(int n = 0; n < bitString.Length; n++) {
                switch(bitString[n]) {
                    case '0': sequence[n] = Bit.Zero; break;
                    case '1': sequence[n] = Bit.One; break;
                    default: throw new ArgumentException();
                }
            }
            return sequence;
        }


        #region Enumerable

        protected override IEnumerator<Bit> CreateEnumerator() {
            for(int n = 0, bits = size; n < data.Length && bits > 0; n++) {
                byte slot = data[n];
                for(int bit = 0; bit < 8 && bits > 0; bit++, bits--) {
                    yield return (slot & 1) == 0 ? Bit.Zero : Bit.One;
                    slot >>= 1;
                }
            }
        }

        #endregion
    }
}