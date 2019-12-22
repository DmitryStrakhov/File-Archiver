using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileArchiver.Base {
    public static class MathHelper {
        public static bool AreEqual(double x, double y) {
            const double Epsilon = 0.000001;
            return Math.Abs(x - y) < Epsilon;
        }
        public static bool AreNotEqual(double x, double y) {
            if(AreEqual(x, y)) return false;
            return true;
        }
    }
}
