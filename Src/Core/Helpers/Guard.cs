using System;

namespace FileArchiver.Core.Helpers {
    public static class Guard {
        public static void IsNotNull(object value, string argument) {
            if(value == null)
                throw new ArgumentNullException(argument);
        }
        public static void IsInRange(int value, int minValue, int maxValue, string argument) {
            if(value < minValue || value > maxValue)
                throw new ArgumentOutOfRangeException(argument);
        }
        public static void IsNotNullOrEmpty(string value, string argument) {
            if(value == null)
                throw new ArgumentNullException(argument);
            if(value == string.Empty)
                throw new ArgumentException(argument);
        }
        public static void IsNotNegative(long value, string argument) {
            if(value < 0)
                throw new ArgumentException(argument);
        }
    }
}