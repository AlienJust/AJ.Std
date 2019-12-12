using System;
using System.Collections;
using System.Collections.Generic;

namespace AlienJust.Support.Collections
{
    /// <summary>
    /// Forms one read-only list from parts of two lists
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    public class StructSubPairList<T> : IReadOnlyList<T> where T : struct
    {
        #region Fields
        private readonly int _startIndex1;
        private readonly int _startIndex2;

        private readonly int _endIndex1;
        private readonly int _endIndex2;

        private readonly int _count;

        private readonly IList<T> _source1;
        private readonly IList<T> _source2;

        private readonly bool _startsInSource2;
        private readonly bool _endsInSource2;
        #endregion
        public StructSubPairList(IList<T> source1, IList<T> source2, int startIndex, int count)
        {
            _source1 = source1;
            _source2 = source2;
            _count = count;

            _startsInSource2 = startIndex >= _source1.Count;
            _endsInSource2 = startIndex + _count > _source1.Count;

            _startIndex1 = _startsInSource2 ? -1 : startIndex; // or some index or end of list
            _startIndex2 = _startsInSource2 ? startIndex - _source1.Count : 0; // or some index or zero

            _endIndex1 = _endsInSource2 ? (_startsInSource2 ? -1 : _source1.Count - 1) : startIndex + _count;
            _endIndex2 = _endsInSource2 ? (_startsInSource2 ? _startIndex2 + count : count - (_endIndex1 - _startIndex1 + 1) - 1) : -1;
        }
        private bool IndexIsInFirstList(int index)
        {
            return _startIndex1 >= 0 && index + _startIndex1 < _source1.Count;
        }

        #region IList<T> Members
        public int IndexOf(T item)
        {
            if (_startIndex1 >= 0)
            {
                for (int i = _startIndex1; i <= _endIndex1; ++i)
                {
                    if (item.Equals(_source1[i]))
                        return i;
                }
            }
            if (_endIndex2 >= 0)
            {
                for (int i = _startIndex2; i <= _endIndex2; ++i)
                {
                    if (item.Equals(_source2[i]))
                        return i;
                }
            }
            return -1;
        }

        public T this[int index]
        {
            get
            {
                if (index >= 0 && index < _count)
                {
                    if (IndexIsInFirstList(index))
                    {
                        return _source1[index + _startIndex1];
                    }
                    if (_startsInSource2)
                    {
                        return _source2[index + _startIndex2];
                    }
                    return _source2[_startIndex2 + index - (_endIndex1 - _startIndex1)]; // TODO: _startIndex2 is allways 0?
                }
                throw new IndexOutOfRangeException("index");
            }
            set
            {
                if (index >= 0 && index < _count)
                {
                    if (IndexIsInFirstList(index))
                    {
                        _source1[index + _startIndex1] = value;
                    }
                    else
                    {
                        if (_startsInSource2)
                            _source2[_startIndex2 + index] = value;
                        else
                            _source2[_startIndex2 + (index - (_endIndex1 - _startIndex1 + 1))] = value; // TODO: _startIndex2 is allways 0?
                    }
                }
                else
                    throw new IndexOutOfRangeException("index");
            }
        }
        #endregion

        #region ICollection<T> Members

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < _count; ++i)
            {
                array[arrayIndex + i] = this[i];
            }
        }
        public int Count => _count;

        public bool IsReadOnly => true;

        #endregion

        #region IEnumerable<T> Members
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = _startIndex1; i < _endIndex1; i++)
            {
                yield return _source1[i];
            }
            for (int i = _startIndex2; i < _endIndex2; i++)
            {
                yield return _source2[i];
            }
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}