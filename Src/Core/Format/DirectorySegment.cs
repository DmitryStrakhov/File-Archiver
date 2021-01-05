using System;
using System.Diagnostics;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.Format {
    [DebuggerDisplay("DirectorySegment(Name={Name})")]
    public sealed class DirectorySegment {
        public DirectorySegment(string name, int cardinality) {
            Guard.IsNotNullOrEmpty(name, nameof(name));
            Guard.IsNotNegative(cardinality, nameof(cardinality));

            this.Name = name;
            this.Cardinality = cardinality;
        }
        public string Name { get; }
        public int Cardinality { get; }
    }
}