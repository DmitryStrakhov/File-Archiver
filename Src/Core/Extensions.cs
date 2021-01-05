using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FileArchiver.Core.Base;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core {
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

    public static class FileSystemEntryEnumeratorExtensions {
        public static bool NextFile(this IEnumerator<FileSystemEntry> @this) {
            while(@this.MoveNext()) {
                if(@this.Current.Type == FileSystemEntryType.File) return true;
            }
            return false;
        }
    }
}