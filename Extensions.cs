using System;
using System.Collections.Generic;
using System.IO;
using FileArchiver.Base;
using FileArchiver.Helpers;
using System.Runtime.CompilerServices;

namespace FileArchiver {
    public static class ObjectExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CastTo<T>(this object @this) {
            return (T)@this;
        }
        public static T Do<T>(this T @this, Action<T> action)
            where T : class {
            if(@this != null) action(@this);
            return @this;
        }
    }

    public static class StringExtensions {
        public static byte[] ToByteArray(this string @this) {
            if(@this == null) return null;

            byte[] result = new byte[@this.Length];
            for(int n = 0; n < @this.Length; n++) {
                result[n] = (byte)@this[n];
            }
            return result;
        }
    }

    public static class ListExtensions {
        public static void Swap<T>(this IList<T> @this, int xPos, int yPos) {
            Guard.IsInRange(xPos, 0, @this.Count - 1, nameof(xPos));
            Guard.IsInRange(yPos, 0, @this.Count - 1, nameof(yPos));
            if(xPos != yPos) {
                T temp = @this[xPos];
                @this[xPos] = @this[yPos];
                @this[yPos] = temp;
            }
        }
    }

    public static class QueueExtensions {
        public static void Enqueue<T>(this Queue<T> @this, IEnumerable<T> items) {
            Guard.IsNotNull(items, nameof(items));

            foreach(T item in items) {
                @this.Enqueue(item);
            }
        }
    }

    public static class StreamExtensions {
        [ThreadStatic] static readonly byte[] buffer = new byte[8];

        public static void WriteLong(this Stream @this, long value) {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            buffer[4] = (byte)(value >> 32);
            buffer[5] = (byte)(value >> 40);
            buffer[6] = (byte)(value >> 48);
            buffer[7] = (byte)(value >> 56);
            @this.Write(buffer, 0, 8);
        }
        public static long ReadLong(this Stream @this) {
            FillBuffer(@this, 8);
            return (long)(uint)(buffer[4] | buffer[5] << 8 | buffer[6] << 16 | buffer[7] << 24) << 32 | (uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
        }
        private static void FillBuffer(Stream stream, int size) {
            int offset = 0;

            do {
                int num = stream.Read(buffer, offset, size - offset);
                if(num == 0)
                    throw new EndOfStreamException();
                offset += num;
            }
            while(offset < size);
        }
    }

    public static class FileSystemEntryEnumeratorExtensions {
        public static bool NextFile(this IEnumerator<FileSystemEntry> @this) {
            while(@this.MoveNext()) {
                if(@this.Current.Type == FileSystemEntryType.File) return true;
            }
            return false;
        }
    }
}