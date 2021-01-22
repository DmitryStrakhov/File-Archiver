using System;
using System.Runtime.CompilerServices;

namespace FileArchiver.Core.DataStructures {
    public sealed class ByteReader {
        byte value;
        int count;

        public ByteReader() {
            this.value = 0;
            this.count = 0;
        }
        public void SetValue(byte val) {
            this.value = val;
            count = 8;
        }
        public Bit ReadBit() {
            if(IsEmpty)
                throw new InvalidOperationException();

            Bit bit = (value & 1) == 0 ? Bit.Zero : Bit.One;
            value >>= 1;
            count--;
            return bit;
        }
        public bool IsEmpty {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return count == 0; }
        }
        public bool IsReady {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return count > 0; }
        }
    }
}
