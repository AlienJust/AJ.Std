using System;
using AJ.Std.Concurrent.Contracts;

namespace AJ.Std.Concurrent
{
	internal sealed class ItemReleaserRelayWithExecutionCountControl<TKey> : IItemsReleaser<TKey> {
		private readonly object _sync;

		private readonly IItemsReleaser<TKey> _originalItemsReleaser;
		private bool _someItemWasReleased;


		public ItemReleaserRelayWithExecutionCountControl(IItemsReleaser<TKey> originalItemsReleaser) {
			_sync = new object();
			_originalItemsReleaser = originalItemsReleaser;
			_someItemWasReleased = false;
		}

		public void ReportSomeAddressedItemIsFree(TKey address) {
			if (!SomeItemWasReleased) {
				_originalItemsReleaser.ReportSomeAddressedItemIsFree(address);
			}
			else {
				throw new Exception("This releaser cannot launch release more than once");
			}
		}

		public bool SomeItemWasReleased {
			get {
				bool result;
				lock (_sync) {
					result = _someItemWasReleased;
				}
				return result;
			}

			private set {
				lock (_sync) {
					_someItemWasReleased = value;
				}
			}
		}
	}
}