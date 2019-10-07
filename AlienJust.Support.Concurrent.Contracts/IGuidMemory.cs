using System;

namespace AlienJust.Support.Concurrent.Contracts {
	public interface IGuidMemory<in T>
	{
		Guid AddObject(T obj);
		void AddObject(Guid guid, T obj);
		void RemoveObject(Guid guid);
	}
}