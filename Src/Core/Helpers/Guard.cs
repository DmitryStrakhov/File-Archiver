using System;
using System.Runtime.CompilerServices;

namespace FileArchiver.Core.Helpers {
    public static class Guard {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNull<T>(T value, string argument)
            where T : class {

            if(value == null)
                throw new ArgumentNullException(argument);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckIndex(int index, int size, string argument) {
            if((uint)index >= (uint)size)
                throw new ArgumentOutOfRangeException(argument);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty(string value, string argument) {
            if(value == null)
                throw new ArgumentNullException(argument);
            if(value == string.Empty)
                throw new ArgumentException(argument);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNegative(int value, string argument) {
            if(value < 0)
                throw new ArgumentException(argument);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNegative(long value, string argument) {
            if(value < 0)
                throw new ArgumentException(argument);
        }
    }
}