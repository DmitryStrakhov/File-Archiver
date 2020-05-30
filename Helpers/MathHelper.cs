using System;

namespace FileArchiver.Helpers {
    public static class MathHelper {
        public static bool AreEqual(double x, double y) {
            const double epsilon = 0.000001;
            return Math.Abs(x - y) < epsilon;
        }
        public static bool AreNotEqual(double x, double y) {
            if(AreEqual(x, y)) return false;
            return true;
        }
        public static bool IsEven(int value) {
            return (value & 1) == 0;
        }
    }
}
