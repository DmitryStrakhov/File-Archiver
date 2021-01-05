using System;
using FileArchiver.Builders;
using FileArchiver.HuffmanCore;
using FileArchiver.DataStructures;
using FileArchiver.Format;

namespace FileArchiver.Parsers {
    public static class FileDecodingInputStreamExtensions {
        public static byte ReadByte(this IDecodingInputStream @this) {
            byte value = 0;

            for(int n = 0; n < 8; n++) {
                if(!@this.ReadBit(out Bit bit))
                    throw new InvalidOperationException();

                if(bit == Bit.One) {
                    value |= (byte)(1 << n);
                }
            }
            return value;
        }
        public static char ReadChar(this IDecodingInputStream @this) {
            byte b1 = @this.ReadByte();
            byte b2 = @this.ReadByte();
            return (char)(b1 | (b2 << 8));
        }
        public static int ReadInt(this IDecodingInputStream @this) {
            byte b1 = @this.ReadByte();
            byte b2 = @this.ReadByte();
            byte b3 = @this.ReadByte();
            byte b4 = @this.ReadByte();
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
        }
        public static long ReadLong(this IDecodingInputStream @this) {
            byte b1 = @this.ReadByte();
            byte b2 = @this.ReadByte();
            byte b3 = @this.ReadByte();
            byte b4 = @this.ReadByte();
            byte b5 = @this.ReadByte();
            byte b6 = @this.ReadByte();
            byte b7 = @this.ReadByte();
            byte b8 = @this.ReadByte();
            return ((long)(uint)(b5 | (b6 << 8) | (b7 << 16) | (b8 << 24)) << 32) | (uint)(b1 | (b2 << 8) | (b3 << 16) | (b4 << 24));
        }

        public static StreamKind ReadStreamFormat(this IDecodingInputStream @this) {
            try { return (StreamKind)@this.ReadByte(); }
            catch(Exception e) { throw new InvalidOperationException(string.Empty, e); }
        }
    }
}