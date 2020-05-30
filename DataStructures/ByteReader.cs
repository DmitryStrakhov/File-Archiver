using System;

namespace FileArchiver.DataStructures {
    public sealed class ByteReader {
        byte value;
        int count;

        public ByteReader() {
            this.value = 0;
            this.count = 0;
        }
        public void SetValue(byte value) {
            this.value = value;
            count = 8;
        }
        public Bit ReadBit() {
            if(!IsReady)
                throw new InvalidOperationException();

            Bit bit = (value & 1) == 0 ? Bit.Zero : Bit.One;
            value >>= 1;
            count--;
            return bit;
        }
        public bool IsReady {
            get { return count > 0; }
        }
    }
}
