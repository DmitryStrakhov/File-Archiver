using System;

namespace FileArchiver.Core.Helpers {
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
        public static int ModAdv(long value, int mod) {
            return (int)((8L - value % mod) % mod);
        }
    }
}
