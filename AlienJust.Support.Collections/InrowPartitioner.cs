using System;
using System.Collections.Generic;
using System.Linq;
using AlienJust.Support.Collections.Contracts;

namespace AlienJust.Support.Collections
{
    public sealed class InrowPartitioner<T> : ISequencePartitioner<T>
    {
        private readonly List<T> _setupSequence; // = new List<EpranSearchResultRow>();
        private int _nextPartitionIndex;
        private readonly int _partitonSize;

        public InrowPartitioner(IEnumerable<T> sequence, int partitionSize)
        {
            if (sequence == null) throw new NullReferenceException("Sequence must be not null");
            if (partitionSize <= 0) throw new ArgumentOutOfRangeException(nameof(partitionSize), "Partition size must be 1 or more");
            _setupSequence = sequence.ToList();
            _partitonSize = partitionSize;
            _nextPartitionIndex = 0;
        }


        public IEnumerable<T> GetNextPart()
        {
            if (_nextPartitionIndex * _partitonSize >= _setupSequence.Count) return null;

            var result = _setupSequence.GetRange(_nextPartitionIndex * _partitonSize, _partitonSize * (_nextPartitionIndex + 1) > _setupSequence.Count ? _setupSequence.Count - _partitonSize * _nextPartitionIndex : _partitonSize);
            _nextPartitionIndex++;

            return result;
        }
    }
}