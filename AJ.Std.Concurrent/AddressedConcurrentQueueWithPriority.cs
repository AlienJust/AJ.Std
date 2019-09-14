using System;
using System.Collections.Concurrent;

namespace AJ.Std.Concurrent
{
    public sealed class AddressedConcurrentQueueWithPriority<TKey, TItem>
    {
        private readonly int _maxPriority;
        private readonly ConcurrentDictionary<TKey, ConcurrentQueueWithPriority<TItem>> _addressedQueues;
        public AddressedConcurrentQueueWithPriority(int maxPriority)
        {
            _maxPriority = maxPriority;
            _addressedQueues = new ConcurrentDictionary<TKey, ConcurrentQueueWithPriority<TItem>>();
        }

        public void Enqueue(TKey key, TItem item, int priority)
        {
            _addressedQueues.GetOrAdd(key, k => new ConcurrentQueueWithPriority<TItem>(_maxPriority)).Enqueue(item, priority);
        }

        public TItem Dequeue(TKey key)
        {
            ConcurrentQueueWithPriority<TItem> addrQueue;
            if (_addressedQueues.TryGetValue(key, out addrQueue))
            {
                return addrQueue.Dequeue();
            }
            throw new Exception("Cannot get queue with priority for " + key);
        }
    }
}