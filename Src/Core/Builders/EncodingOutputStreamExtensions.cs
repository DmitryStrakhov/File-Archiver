using System;
using FileArchiver.HuffmanCore;
using FileArchiver.DataStructures;
using FileArchiver.Format;

namespace FileArchiver.Builders {
    public static class EncodingOutputStreamExtensions {
        public static void Write(this IEncodingOutputStream @this, StreamKind value) {
            Write(@this, (byte)value);
        }
        public static void Write(this IEncodingOutputStream @this, byte value) {
            for(int n = 0; n < 8; n++) {
                @this.WriteBit((value & 1) == 1 ? Bit.One : Bit.Zero);
                value >>= 1;
            }
        }
        public static void Write(this IEncodingOutputStream @this, char symbol) {
            for(int n = 0; n < 2; n++) {
                Write(@this, (byte)symbol);
                symbol >>= 8;
            }
        }
        public static void Write(this IEncodingOutputStream @this, int value) {
            for(int n = 0; n < 4; n++) {
                Write(@this, (byte)value);
                value >>= 8;
            }
        }
        public static void Write(this IEncodingOutputStream @this, long value) {
            for(int n = 0; n < 8; n++) {
                Write(@this, (byte)value);
                value >>= 8;
            }
        }
    }
}