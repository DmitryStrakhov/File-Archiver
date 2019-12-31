using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileArchiver.Extensions {
    public static class ObjectExtensions {
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
