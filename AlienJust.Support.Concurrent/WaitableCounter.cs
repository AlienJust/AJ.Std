using System;
using System.Threading;

namespace AlienJust.Support.Concurrent
{
	/// <summary>
	/// Counter with methods to wait for counting
	/// </summary>
	public sealed class WaitableCounter {
		private readonly object _sync = new object();
		private int _count;

		private readonly AutoResetEvent _incrementSignal;
		private readonly AutoResetEvent _decrementSignal;
		private readonly AutoResetEvent _changeSignal;

		public WaitableCounter(int count) {
			_count = count;
			_incrementSignal = new AutoResetEvent(false);
			_decrementSignal = new AutoResetEvent(false);
			_changeSignal = new AutoResetEvent(false);
		}

		public WaitableCounter() {
			_incrementSignal = new AutoResetEvent(false);
			_decrementSignal = new AutoResetEvent(false);
			_changeSignal = new AutoResetEvent(false);
		}

		public void IncrementCount() {
			lock (_sync) {
				_count += 1;
				_changeSignal.Set();
				_incrementSignal.Set();
			}
		}

		public void DecrementCount() {
			lock (_sync) {
				_count -= 1;
				_changeSignal.Set();
				_decrementSignal.Set();
			}
		}

		/// <summary>
		/// Check count is equal to argument
		/// </summary>
		/// <returns>True if count value is equal to checked one</returns>
		public bool CompareCount(int compareTo) {
			lock (_sync) {
				return _count == compareTo;
			}
		}

		public int Count {
			get {
				lock (_sync) {
					return _count;
				}
			}
			set {
				lock (_sync) {
					_count = value;
					_changeSignal.Set();
					// TODO: maybe set decrement and increment waiters?
				}
			}
		}

		public void WaitForIncrement() {
			_incrementSignal.WaitOne();
		}

		public void WaitForDecrement() {
			_decrementSignal.WaitOne();
		}

		public void WaitForCounterChangeWhileNotPredecate(Func<int, bool> checkFunc) {
			while (true) {
				bool exit;
				// first we check, then wait 
				// lock is needed for not to skip any .Set() call (it's call is also locked on _sync object)
				lock (_sync) {
					exit = checkFunc(Count);
				}
				if (exit) break;
				_changeSignal.WaitOne();
			}
		}

		public void ActOnLockedCounterAndIncrementCount(Action<int> actionOnLockedCount) {
			lock (_sync) {
				actionOnLockedCount(_count);
				_count += 1;
				_changeSignal.Set();
				_incrementSignal.Set();
			}
		}

		public void ActOnLockedCounterAndDecrementCount(Action<int> actionOnLockedCount) {
			lock (_sync) {
				actionOnLockedCount(_count);
				_count -= 1;
				_changeSignal.Set();
				_decrementSignal.Set();
			}
		}
	}
}