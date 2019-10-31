using System;
using System.Windows.Threading;
using AlienJust.Support.Concurrent.Contracts;

namespace AlienJust.Support.Wpf
{
	public sealed class WpfUiNotifier : IThreadNotifier
	{
		private readonly Dispatcher _dispatcher;

		public WpfUiNotifier(Dispatcher dispatcher)
		{
			_dispatcher = dispatcher;
		}

		public void Notify(Action notifyAction)
		{
			_dispatcher.Invoke(notifyAction);
		}
	}
}
