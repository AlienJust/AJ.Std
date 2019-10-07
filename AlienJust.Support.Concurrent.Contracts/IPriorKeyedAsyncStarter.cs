using System;

namespace AlienJust.Support.Concurrent.Contracts {
	public interface IPriorKeyedAsyncStarter<in TAddressKey>
	{
		Guid AddWork(Action<Action> asyncAction, int priority, TAddressKey key);
		bool RemoveExecution(Guid id);
	}
}