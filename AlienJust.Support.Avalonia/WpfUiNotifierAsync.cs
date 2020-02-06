using System;
using System.Windows.Threading;
using AlienJust.Support.Concurrent.Contracts;

namespace AlienJust.Support.Wpf {
	public sealed class WpfUiNotifierAsync : IThreadNotifier {
		private readonly Dispatcher _dispatcher;

		public WpfUiNotifierAsync(Dispatcher dispatcher) {
			_dispatcher = dispatcher;
		}

		public void Notify(Action notifyAction) {
			_dispatcher.BeginInvoke(notifyAction);
		}
	}
}