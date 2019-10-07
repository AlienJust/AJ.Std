using System;

namespace AlienJust.Support.Concurrent.Contracts {
	public interface IThreadNotifier
	{
		void Notify(Action notifyAction);
	}
}