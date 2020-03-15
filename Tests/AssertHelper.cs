using System;
using FileArchiver.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    public static class AssertHelper {
        public static void AreEqual(double expected, double actual) {
            if(MathHelper.AreNotEqual(expected, actual)) Assert.Fail();
        }
    }
}