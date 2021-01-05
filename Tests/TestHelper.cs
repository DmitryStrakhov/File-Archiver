using System;
using FileArchiver.Core.DataStructures;

namespace FileArchiver.Tests {
    public static class TestHelper {
        public static Bit[] BitsFromString(string @string) {
            Bit[] result = new Bit[@string.Length];

            for(int n = 0; n < @string.Length; n++) {
                switch(@string[n]) {
                    case '0':
                        result[n] = Bit.Zero;
                        break;
                    case '1':
                        result[n] = Bit.One;
                        break;
                    default: throw new ArgumentException();
                }
            }
            return result;
        }
        public static string StringFromBits(Bit[] bits) {
            char[] chars = new char[bits.Length];

            for(int n = 0; n < bits.Length; n++) {
                chars[n] = bits[n] == Bit.Zero ? '0' : '1';
            }
            return new string(chars);
        }
    }
}
