using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FileArchiver.Core.DataStructures {
    [DebuggerDisplay("Pair(X={X}, Y={Y})")]
    public struct Pair<TX, TY> : IEquatable<Pair<TX, TY>> {
        public readonly TX X;
        public readonly TY Y;

        public Pair(TX x, TY y) {
            this.X = x;
            this.Y = y;
        }

        #region Equals & GetHashCode

        public bool Equals(Pair<TX, TY> other) {
            return EqualityComparer<TX>.Default.Equals(X, other.X) && EqualityComparer<TY>.Default.Equals(Y, other.Y);
        }
        public override bool Equals(object obj) {
            Pair<TX, TY> other = (Pair<TX, TY>)obj;
            return Equals(other);
        }
        public override int GetHashCode() {
            return 0;
        }

        #endregion
    }
}