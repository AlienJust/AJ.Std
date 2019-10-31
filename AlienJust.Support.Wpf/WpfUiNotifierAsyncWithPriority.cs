using System;
using System.Windows.Threading;
using AlienJust.Support.Concurrent.Contracts;

namespace AlienJust.Support.Wpf
{
	public sealed class WpfUiNotifierAsyncWithPriority : IThreadNotifier {
		private readonly Dispatcher _dispatcher;
		private readonly DispatcherPriority _priority;

		public WpfUiNotifierAsyncWithPriority(Dispatcher dispatcher, DispatcherPriority priority)
		{
			_dispatcher = dispatcher;
			_priority = priority;
		}

		public void Notify(Action notifyAction) {
			_dispatcher.BeginInvoke(notifyAction, _priority);
		}
	}
}