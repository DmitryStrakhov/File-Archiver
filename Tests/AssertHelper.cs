#if DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FileArchiver.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArchiver.Tests {
    public static class AssertHelper {
        public static void AreEqual(double expected, double actual) {
            if(MathHelper.AreNotEqual(expected, actual)) Assert.Fail();
        }
        public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual) {
            if(expected == null || actual == null) {
                throw new AssertFailedException();
            }
            CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray());
        }
        public static void AreEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual) {
            if(expected == null || actual == null) {
                throw new AssertFailedException();
            }
            CollectionAssert.AreEquivalent(expected.ToArray(), actual.ToArray());
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
        public static void CollectionIsEmpty(IEnumerable collection) {
            if(collection == null) throw new AssertFailedException();

            foreach(object item in collection) {
                throw new AssertFailedException();
            }
        }
        public static void CollectionIsEmpty(ICollection collection) {
            if(collection == null || collection.Count != 0) throw new AssertFailedException();
        }
    }
}
#endif