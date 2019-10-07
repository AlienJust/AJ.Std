using System;
using System.Collections;
using System.Collections.Generic;

namespace AlienJust.Support.Collections
{
	/// <summary>
	/// Forms sublist of value type from another list
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	public class StructSubList<T> : IReadOnlyList<T> where T : struct {
		#region Fields

		private readonly int _startIndex;
		private readonly int _endIndex;
		private readonly int _count;
		private readonly IList<T> _source;

		#endregion

		public StructSubList(IList<T> source, int startIndex, int count) {
			_source = source;
			_startIndex = startIndex;
			_count = count;
			_endIndex = _startIndex + _count - 1;
		}

		#region IList<T> Members

		public int IndexOf(T item) {
			for (int i = _startIndex; i <= _endIndex; i++) {
				if (item.Equals(_source[i]))
					return i;
			}
			return -1;
		}

		public T this[int index] {
			get {
				if (index >= 0 && index < _count)
					return _source[index + _startIndex];
				throw new IndexOutOfRangeException("index");
			}
			set {
				if (index >= 0 && index < _count)
					_source[index + _startIndex] = value;
				else
					throw new IndexOutOfRangeException("index");
			}
		}

		#endregion

		#region ICollection<T> Members

		public bool Contains(T item) {
			return IndexOf(item) >= 0;
		}

		public void CopyTo(T[] array, int arrayIndex) {
			for (int i = 0; i < _count; i++) {
				array[arrayIndex + i] = _source[i + _startIndex];
			}
		}

		public int Count => _count;

		public bool IsReadOnly => true;

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator() {
			for (int i = _startIndex; i < _endIndex; i++) {
				yield return _source[i];
			}
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion
	}
}