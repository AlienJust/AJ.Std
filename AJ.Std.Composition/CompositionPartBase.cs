using AJ.Std.Composition.Contracts;

namespace AJ.Std.Composition
{
	public abstract class CompositionPartBase : ICompositionPart {
		private readonly IUnknownLike _lifetime;

		protected CompositionPartBase() {
			_lifetime = new UnknownLikeLocked();
		}

		public void Release() {
			_lifetime.Release();
			if (_lifetime.RefsCount == 0) {
				BecameUnused();
			}
		}

		public void AddRef() {
			_lifetime.AddRef();
		}

		public int RefsCount => _lifetime.RefsCount;
		public abstract string Name { get; }
		public abstract void SetCompositionRoot(ICompositionRoot root);
		public abstract void BecameUnused();
	}
}
