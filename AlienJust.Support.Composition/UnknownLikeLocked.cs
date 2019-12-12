using AlienJust.Support.Composition.Contracts;

namespace AlienJust.Support.Composition
{
    public class UnknownLikeLocked : IUnknownLike
    {
        private int _refsCount;
        private readonly object _syncRefsCount;

        public UnknownLikeLocked()
        {
            _refsCount = 0;
            _syncRefsCount = new object();
        }
        public void Release()
        {
            lock (_syncRefsCount)
                _refsCount--;
        }

        public void AddRef()
        {
            lock (_syncRefsCount)
                _refsCount++;
        }

        public int RefsCount
        {
            get { lock (_syncRefsCount) return _refsCount; }
        }
    }
}