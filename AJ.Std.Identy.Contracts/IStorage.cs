using System.Collections.Generic;

namespace AJ.Std.Identy.Contracts {
	// move to project AJ.Std.Store when growing
	public interface IStorage<TDataItem> where TDataItem : IObjectWithIdentifier {
		IEnumerable<TDataItem> StoredItems { get; }
		void Add(TDataItem item);
		void Remove(TDataItem item);
		void Update(IIdentifier id, TDataItem item);
	}
}
