using System;
using System.Collections;
using FileArchiver.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    public static class AssertHelper {
        public static void AreEqual(double expected, double actual) {
            if(MathHelper.AreNotEqual(expected, actual)) Assert.Fail();
        }
        public static void Throws<T>(Action action) where T : Exception {
            Guard.IsNotNull(action, nameof(action));
            Exception exception = null;

            try {
                action();
            }
            catch(Exception e) {
                exception = e;
            }
            if(!(exception is T)) throw new AssertFailedException();
        }
        public static void CollectionIsEmpty(ICollection collection) {
            if(collection == null || collection.Count != 0) throw new AssertFailedException();
        }
    }
}