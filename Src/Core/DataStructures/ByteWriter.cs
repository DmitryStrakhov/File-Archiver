using System;

namespace FileArchiver.Core.DataStructures {
    public sealed class ByteWriter {
        byte value;
        int index;

        public ByteWriter() {
            this.value = 0;
            this.index = 0;
        }
        public void AddBit(Bit bit) {
            if(IsReady)
                throw new InvalidOperationException();

            if(bit == Bit.One) {
                value |= (byte)(1 << index);
            }
            index++;
        }
        public void Reset() {
            value = 0;
            index = 0;
        }
        public bool IsEmpty {
            get { return index == 0; }
        }
        public bool IsReady {
            get { return index == 8; }
        }
        public byte Value { get { return value; } }
    }
}