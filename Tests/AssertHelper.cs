using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileArchiver.Helpers;
using NUnit.Framework;

namespace FileArchiver.Tests {
    public static class AssertHelper {
        public static void AreEqual(double expected, double actual) {
            if(MathHelper.AreNotEqual(expected, actual)) Assert.Fail();
        }
        public static void TrueForAll<T>(ICollection<T> collection, int expectedSize, Predicate<T> predicate) {
            if(collection == null || predicate == null) throw new AssertionException(nameof(collection) + " is null");

            if(collection.Count != expectedSize)
                throw new AssertionException(nameof(collection));
            foreach(T item in collection) {
                if(!predicate(item))
                    throw new AssertionException(nameof(collection));
            }
        }
        public static void StringIsEmpty(string actual) {
            if(!string.IsNullOrEmpty(actual))
                throw new AssertionException("string is not empty");
        }
        public static void AssertStream(MemoryStream stream, params byte[] expected) {
            if(stream == null) throw new AssertionException(nameof(stream) + " is null");
            CollectionAssert.AreEqual(expected, stream.ToArray());
        }
        public static void StreamIsEmpty(MemoryStream stream) {
            if(stream == null || stream.ToArray().Length != 0)
                throw new AssertionException(nameof(stream) + " is not empty");
        }
    }
}