using System;
using System.Collections.Generic;

namespace TileKit.Core.Path
{
    /// <summary>Simple binary min-heap priority queue.</summary>
    public sealed class PriorityQueue<T>
    {
        private readonly List<(T item, int priority)> _heap = new List<(T,int)>();
        private readonly IComparer<int> _cmp = Comparer<int>.Default;

        public int Count => _heap.Count;

        public void Enqueue(T item, int priority)
        {
            _heap.Add((item, priority));
            SiftUp(_heap.Count - 1);
        }

        public bool TryDequeue(out T item, out int priority)
        {
            if (_heap.Count == 0) { item = default!; priority = 0; return false; }
            (item, priority) = _heap[0];
            var last = _heap[^1];
            _heap.RemoveAt(_heap.Count - 1);
            if (_heap.Count > 0)
            {
                _heap[0] = last;
                SiftDown(0);
            }
            return true;
        }

        private void SiftUp(int i)
        {
            while (i > 0)
            {
                int p = (i - 1) >> 1;
                if (_cmp.Compare(_heap[i].priority, _heap[p].priority) >= 0) break;
                (_heap[i], _heap[p]) = (_heap[p], _heap[i]);
                i = p;
            }
        }

        private void SiftDown(int i)
        {
            int n = _heap.Count;
            while (true)
            {
                int l = (i << 1) + 1, r = l + 1, s = i;
                if (l < n && _cmp.Compare(_heap[l].priority, _heap[s].priority) < 0) s = l;
                if (r < n && _cmp.Compare(_heap[r].priority, _heap[s].priority) < 0) s = r;
                if (s == i) break;
                (_heap[i], _heap[s]) = (_heap[s], _heap[i]);
                i = s;
            }
        }
    }
}