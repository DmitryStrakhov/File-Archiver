﻿using System;
using System.Text;
using System.Runtime.CompilerServices;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Tests {
    public interface ITraceableObject {
        void TraceMember(string name);
        string GetTrace();
    }

    public abstract class TraceableObject : ITraceableObject {
        readonly ITraceableObject owner;
        readonly StringBuilder traceBuilder;

        protected TraceableObject(ITraceableObject owner) {
            Guard.IsNotNull(owner, nameof(owner));
            this.owner = owner;
            this.traceBuilder = new StringBuilder(64);
        }

        public void TraceMember([CallerMemberName] string name = null) {
            traceBuilder.Append(name);
            traceBuilder.Append(";");
            owner.TraceMember(name);
        }
        public string GetTrace() {
            return traceBuilder.ToString();
        }
    }

    public sealed class NoneTraceableObject : ITraceableObject {
        private NoneTraceableObject() {
        }
        public static readonly NoneTraceableObject Instance = new NoneTraceableObject();

        public void TraceMember(string name) {
        }
        public string GetTrace() {
            return string.Empty;
        }
    }
}
