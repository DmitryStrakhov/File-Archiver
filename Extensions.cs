using System;
using System.Runtime.CompilerServices;

namespace FileArchiver.Extensions {
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
}