using System;

namespace AJ.Std.Concurrent.Contracts {
	public interface IThreadNotifier
	{
		void Notify(Action notifyAction);
	}
}