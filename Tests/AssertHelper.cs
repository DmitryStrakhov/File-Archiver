﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public static void StringIsEmpty(string actual) {
            if(!string.IsNullOrEmpty(actual))
                throw new AssertFailedException("was " + actual);
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
        public static void TrueForAll<T>(ICollection<T> collection, int expectedSize, Predicate<T> predicate) {
            if(collection == null || predicate == null) throw new AssertFailedException();

            if(collection.Count != expectedSize)
                throw new AssertFailedException();
            foreach(T item in collection) {
                if(!predicate(item))
                    throw new AssertFailedException();
            }
        }
        public static void AssertStream(MemoryStream stream, params byte[] expected) {
            if(stream == null) throw new AssertFailedException();

            AreEqual(expected, stream.ToArray());
        }
        public static void StreamIsEmpty(MemoryStream stream) {
            if(stream == null || stream.ToArray().Length != 0)
                throw new AssertFailedException();
        }
    }
}
