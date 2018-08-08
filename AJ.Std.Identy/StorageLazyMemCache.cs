using System;
using System.Collections.Generic;
using System.Linq;
using AJ.Std.Identy.Contracts;

namespace AJ.Std.Identy {
  public class StorageLazyMemCache<TDataItem> : IStorage<TDataItem> where TDataItem : IObjectWithIdentifier {
    private readonly IStorage<TDataItem> _originalStorage;
    private readonly Lazy<List<TDataItem>> _lazyItems;

    public StorageLazyMemCache(IStorage<TDataItem> originalStorage) {
      _originalStorage = originalStorage;
      _lazyItems = new Lazy<List<TDataItem>>(() => _originalStorage.StoredItems.ToList());
    }

    public IEnumerable<TDataItem> StoredItems => _lazyItems.Value;

    public void Add(TDataItem item) {
      _originalStorage.Add(item);
      _lazyItems.Value.Add(item);
    }

    public void Remove(TDataItem item) {
      _originalStorage.Remove(item);
      var valuesToRemove = _lazyItems.Value.Where(dataItem => item.Id.IdentyString == dataItem.Id.IdentyString).ToList();
      foreach (var dataItem in valuesToRemove) {
        _lazyItems.Value.Remove(dataItem);
      }
    }

    public void Update(IIdentifier id, TDataItem item) {
      _originalStorage.Update(id, item);
      for (int i = 0; i < _lazyItems.Value.Count; ++i) {
        if (_lazyItems.Value[i].Id.IdentyString == id.IdentyString)
          _lazyItems.Value[i] = item;
      }
    }
  }
}