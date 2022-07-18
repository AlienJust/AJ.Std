using System;
using System.Collections.Concurrent;
using System.Threading;
using AlienJust.Support.Concurrent.Contracts;

namespace AlienJust.Support.Concurrent
{
    public sealed class SingleThreadedRelayQueueWorkerProceedAllItemsBeforeStopNoLog<TItem> : IWorker<TItem>, IStoppableWorker
    {
        private readonly object _syncUserActions;
        private readonly object _syncRunFlags;
        private readonly ConcurrentQueue<TItem> _items;
        private readonly string _name; // TODO: implement interface INamedObject
        private readonly Action<TItem> _action;
        private readonly Thread _workThread;

        private bool _isRunning;
        private bool _mustBeStopped;

        private readonly AutoResetEvent _threadNotifyAboutQueueItemsCountChanged;

        public SingleThreadedRelayQueueWorkerProceedAllItemsBeforeStopNoLog(string name, Action<TItem> action, ThreadPriority threadPriority, bool markThreadAsBackground, ApartmentState? apartmentState)
        {
            _syncRunFlags = new object();
            _syncUserActions = new object();

            _items = new ConcurrentQueue<TItem>();
            _name = name;
            _action = action ?? throw new ArgumentNullException(nameof(action));

            _threadNotifyAboutQueueItemsCountChanged = new AutoResetEvent(false);

            _isRunning = true;
            _mustBeStopped = false;

            _workThread = new Thread(WorkingThreadStart) { IsBackground = markThreadAsBackground, Priority = threadPriority, Name = name };
            if (apartmentState.HasValue) _workThread.SetApartmentState(apartmentState.Value);
            _workThread.Start();
        }

        public void AddWork(TItem workItem)
        {
            lock (_syncUserActions)
            {
                lock (_syncRunFlags)
                {
                    if (!_mustBeStopped)
                    {
                        _items.Enqueue(workItem);
                        _threadNotifyAboutQueueItemsCountChanged.Set();
                    }
                    else
                    {
                        var ex = new Exception("Cannot handle items any more, worker has been stopped or stopping now");
                        throw ex;
                    }
                }
            }
        }

        private void WorkingThreadStart()
        {
            IsRunning = true;
            try
            {
                while (true)
                {
                    while (_items.TryDequeue(out var dequeuedItem))
                    {
                        try
                        {
                            _action(dequeuedItem);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    if (MustBeStopped) throw new Exception("MustBeStopped is true, this is the end of thread");
                    _threadNotifyAboutQueueItemsCountChanged.WaitOne();
                }
            }
            catch
            {
                IsRunning = false;
            }
        }

        public void StopAsync()
        {
            lock (_syncUserActions)
            {
                lock (_syncRunFlags)
                {
                    _mustBeStopped = true;
                }
                _threadNotifyAboutQueueItemsCountChanged.Set();
            }
        }

        public void WaitStopComplete()
        {
            while (!_workThread.Join(200))
            {
                _threadNotifyAboutQueueItemsCountChanged.Set();
            }
        }


        public bool IsRunning
        {
            get
            {
                bool result;
                lock (_syncRunFlags)
                {
                    result = _isRunning;
                }
                return result;
            }

            private set
            {
                lock (_syncRunFlags)
                {
                    _isRunning = value;
                }
            }
        }

        private bool MustBeStopped
        {
            get
            {
                bool result;
                lock (_syncRunFlags)
                {
                    result = _mustBeStopped;
                }
                return result;
            }
        }
    }
}