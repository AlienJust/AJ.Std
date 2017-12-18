using System;
using System.Collections;
using System.Collections.Generic;

namespace AJ.Std.Collections {
	/// <summary>
	/// Forms one list of structures from two lists
	/// </summary>
	/// <typeparam name="T">Value type (value class)</typeparam>
	public class StructPairList<T> : IReadOnlyList<T> where T : struct {
		#region Fields

		private readonly int _count;

		private readonly IList<T> _source1;
		private readonly IList<T> _source2;

		#endregion

		public StructPairList(IList<T> source1, IList<T> source2) {
			_source1 = source1;
			_source2 = source2;
			_count = source1.Count + source2.Count;
		}

		private bool IndexIsInFirstList(int index) {
			return index < _source1.Count;
		}

		#region IList<T> Members

		public int IndexOf(T item) {
			for (int i = 0; i < _count; ++i) {
				if (item.Equals(this[i]))
					return i;
			}
			return -1;
		}

		//public void Insert(int index, T item) {
		//throw new NotSupportedException();
		//}

		//public void RemoveAt(int index) {
		//throw new NotSupportedException();
		//}

		public T this[int index] {
			get {
				if (index >= 0 && index < _count) {
					if (IndexIsInFirstList(index)) {
						return _source1[index];
					}
					return _source2[index - _source1.Count];
				}
				throw new IndexOutOfRangeException("index");
			}
			set {
				if (index >= 0 && index < _count) {
					if (IndexIsInFirstList(index)) {
						_source1[index] = value;
					}
					else {
						_source2[index - _source1.Count] = value;
					}
				}
				else
					throw new IndexOutOfRangeException("index");
			}
		}

		#endregion

		#region ICollection<T> Members

		//public void Add(T item) {
		//throw new NotSupportedException();
		//}

		//public void Clear() {
		//throw new NotSupportedException();
		//}

		public bool Contains(T item) {
			return IndexOf(item) >= 0;
		}

		public void CopyTo(T[] array, int arrayIndex) {
			for (int i = 0; i < _count; ++i) {
				array[arrayIndex + i] = this[i];
			}
		}

		public int Count => _count;

		public bool IsReadOnly => true;

		//public bool Remove(T item) {
		//throw new NotSupportedException();
		//}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator() {
			for (int i = 0; i < _count; ++i) {
				yield return this[i];
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