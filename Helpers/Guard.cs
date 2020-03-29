using System;

namespace FileArchiver.Helpers {
    public static class Guard {
        public static void IsTrue(bool value, string argument) {
            if(!value)
                throw new ArgumentException(argument);
        }
        public static void IsFalse(bool value, string argument) {
            if(value)
                throw new ArgumentException(argument);
        }
        public static void Fail(string message) {
            throw new Exception(message);
        }
        public static void IsNotNull(object value, string argument) {
            if(value == null)
                throw new ArgumentNullException(argument);
        }
    }
}
