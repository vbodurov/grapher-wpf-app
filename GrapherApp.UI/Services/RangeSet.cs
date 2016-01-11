using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GrapherApp.UI.Services
{
    

    public class RangeSet<T> : IEnumerable<T>
    {
        private readonly List<RangeState<T>> _ranges = new List<RangeState<T>>();  
        private readonly double _from;
        private RangeState<T> _lastRangeState;

        public RangeSet(double from)
        {
            _from = from;
        }

        public double LastLimit
        {
            get
            {
                if (_ranges.Count == 0) return _from;
                return _ranges[_ranges.Count - 1].To;
            }
        }

        public T LastValue
        {
            get
            {
                if (_ranges.Count == 0) return default(T);
                return _ranges[_ranges.Count - 1].Value;
            }
        }

        public int Count { get { return _ranges.Count; } }

        public void Add(double to, T state)
        {
            var from = _from;
            if (_ranges.Count > 0)
            {
                from = _ranges[_ranges.Count - 1].To;
            }
            if (to <= from)
            {
                throw new ArgumentException("To value overlaps with existing ranges");
            }
            _ranges.Add(new RangeState<T>
                        {
                            Index = _ranges.Count,
                            From = from,
                            To = to,
                            Value = state
                        });
        }

        public bool TryFind(double n, out T state)
        {
            // first we check previously requested range
            if (_lastRangeState != null)
            {
                if (_lastRangeState.IsWithin(n))
                {
                    state = _lastRangeState.Value;
                    return true;
                }
                // if it isn't last check the next
                if(_lastRangeState.Index < (_ranges.Count - 1) && _ranges[_lastRangeState.Index+1].IsWithin(n))
                {
                    _lastRangeState = _ranges[_lastRangeState.Index + 1];
                    state = _lastRangeState.Value;
                    return true;
                }
            }
            // if no ranges return NULL
            if (_ranges.Count == 0)
            {
                state = default(T);
                return false;
            }
            
            var first = _ranges[0];
            // check the first - in most cases the first call will ask for the first range
            if (first.IsWithin(n))
            {
                _lastRangeState = first;
                state = _lastRangeState.Value;
                return true;
            }
            // if less than all return NULL
            if (n < first.From)
            {
                state = default(T);
                return false;
            }
            // if more than all return NULL
            var last = _ranges[0];
            if (n > last.To)
            {
                state = default(T);
                return false;
            }
            var min = 0;
            var max = _ranges.Count - 1;
            var index = GetMiddleIndex(min, max);
            var current = _ranges[index];
            // binary search
            for (var i = 0; i < 512; ++i)
            {
                if (current.IsWithin(n))
                {
                    state = current.Value;
                    _lastRangeState = current;
                    return true;
                }
                if(n < current.From)
                {
                    max = current.Index - 1;
                    index = GetMiddleIndex(min, max);
                    current = _ranges[index];
                }
                else if (n > current.To)
                {
                    min = current.Index + 1;
                    index = GetMiddleIndex(min, max);
                    current = _ranges[index];
                }
            }
            state = default(T);
            return false;
        }

        private int GetMiddleIndex(int min, int max)
        {
            if (min < 0) min = 0;
            if (max <= min) return min;
            return min + (int) ((max - min)*0.5);
        }

        public void Clear()
        {
            _ranges.Clear();
            _lastRangeState = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _ranges.Select(rs => rs.Value).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal class RangeState<T1>
        {
            public int Index { get; set; }
            public double From { get; set; }
            public double To { get; set; }
            public T1 Value { get; set; }
            public bool IsWithin(double n)
            {
                return n >= From && n <= To;
            }
        }
    }
}