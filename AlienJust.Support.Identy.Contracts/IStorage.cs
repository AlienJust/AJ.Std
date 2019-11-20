using System.Collections.Generic;

namespace AlienJust.Support.Identy.Contracts
{
    // move to project AlienJust.Support.Store when growing
    public interface IStorage<TDataItem> where TDataItem : IObjectWithIdentifier
    {
        IEnumerable<TDataItem> StoredItems { get; }
        void Add(TDataItem item);
        void Remove(TDataItem item);
        void Update(IIdentifier id, TDataItem item);
    }
}
