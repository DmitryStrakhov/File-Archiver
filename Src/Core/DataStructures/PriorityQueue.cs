using System;
using System.Collections.Generic;
using System.Diagnostics;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.DataStructures {
    [DebuggerDisplay("MinimumPriorityQueue(Size={heap.Size})")]
    public class MinimumPriorityQueue<TKey, TValue> where TKey : IComparable<TKey> {
        readonly MinBinaryHeap<TKey, TValue> heap;

        protected MinimumPriorityQueue(int capacity) {
            this.heap = new MinBinaryHeap<TKey, TValue>(capacity);
        }

        public void Insert(TKey key, TValue value) {
            heap.Insert(key, value);
        }
        public TValue GetMinimumValue() {
            return heap.GetMinimumValue();
        }
        public TValue DeleteMinimumValue() {
            return heap.DeleteMinimumValue();
        }
        public int Size { get { return heap.Size; } }
    }


    [DebuggerDisplay("MinBinaryHeap(Size={Size})")]
    public class MinBinaryHeap<TKey, TValue> where TKey : IComparable<TKey> {
        KeyValuePair<TKey, TValue>[] items;
        int size;
        int capacity;

        public MinBinaryHeap()
            : this(4) {
        }
        public MinBinaryHeap(int capacity) {
            this.size = 0;
            this.capacity = capacity;
            this.items = new KeyValuePair<TKey, TValue>[capacity];
        }

        public void Insert(TKey key, TValue value) {
            int pos = Size;
            EnsureSize(Size + 1);
            size++;
            items[pos] = new KeyValuePair<TKey, TValue>(key, value);
            PercolateUp(pos);
        }
        public TValue GetMinimumValue() {
            if(Size == 0) throw new InvalidOperationException();
            return items[0].Value;
        }
        public TValue DeleteMinimumValue() {
            if(Size == 0) throw new InvalidOperationException();
            TValue result = items[0].Value;
            items.Swap(0, Size - 1);
            size--;
            if(Size != 0) PercolateDown(0);
            return result;
        }
        public void Clear() {
            size = 0;
        }
        public int Size {
            get { return size; }
        }

        private void EnsureSize(int size) {
            if(size > capacity) {
                int _capacity = capacity * 2;
                if(size > _capacity)
                    _capacity = size * 2;
                KeyValuePair<TKey, TValue>[] _items = new KeyValuePair<TKey, TValue>[_capacity];
                Array.Copy(items, _items, Size);
                items = _items;
                capacity = _capacity;
            }
        }
        private int PercolateDown(int pos) {
            return DoPercolateDown(items, Size, pos, (keyPos, childPos) => Comparer<TKey>.Default.Compare(items[keyPos].Key, items[childPos].Key) < 0);
        }
        private int PercolateUp(int pos) {
            return DoPercolateUp(items, Size, pos, (keyPos, parentPos) => Comparer<TKey>.Default.Compare(items[keyPos].Key, items[parentPos].Key) > 0);
        }

        static int GetParent(int size, int pos) {
            Guard.CheckIndex(pos, size, nameof(pos));
            return pos != 0 ? (pos - 1) / 2 : -1;
        }
        static int GetLeftChild(int size, int pos) {
            Guard.CheckIndex(pos, size, nameof(pos));
            int result = 2 * pos + 1;
            if(result >= size) result = -1;
            return result;
        }
        static int GetRightChild(int size, int pos) {
            Guard.CheckIndex(pos, size, nameof(pos));
            int result = 2 * pos + 2;
            if(result >= size) result = -1;
            return result;
        }
        static int DoPercolateDown(KeyValuePair<TKey, TValue>[] heapData, int size, int pos, Func<int, int, bool> getIsRightPosition) {
            Guard.CheckIndex(pos, size, nameof(pos));
            int keyPos = pos;
            while(true) {
                int minKeyPos = keyPos;
                int lChildPos = GetLeftChild(size, keyPos);
                int rChildPos = GetRightChild(size, keyPos);

                if(lChildPos != -1 && !getIsRightPosition(minKeyPos, lChildPos)) {
                    minKeyPos = lChildPos;
                }
                if(rChildPos != -1 && !getIsRightPosition(minKeyPos, rChildPos)) {
                    minKeyPos = rChildPos;
                }
                if(minKeyPos == keyPos) return minKeyPos;
                heapData.Swap(keyPos, minKeyPos);
                keyPos = minKeyPos;
            }
        }
        static int DoPercolateUp(KeyValuePair<TKey, TValue>[] heapData, int size, int pos, Func<int, int, bool> getIsRightPosition) {
            Guard.CheckIndex(pos, size, nameof(pos));
            int keyPos = pos;
            while(true) {
                int parentPos = GetParent(size, keyPos);
                if(parentPos == -1 || getIsRightPosition(keyPos, parentPos)) {
                    return keyPos;
                }
                heapData.Swap(parentPos, keyPos);
                keyPos = parentPos;
            }
        }
    }
}