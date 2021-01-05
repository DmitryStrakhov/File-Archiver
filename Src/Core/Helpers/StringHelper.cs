using System;

namespace FileArchiver.Core.Helpers {
    public static class StringHelper {
        public static bool AreEqual(string x, string y) {
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}