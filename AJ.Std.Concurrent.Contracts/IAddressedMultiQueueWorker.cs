using System;

namespace AJ.Std.Concurrent.Contracts
{
	/// <summary>
	/// Worker that uses queue with priority
	/// </summary>
	/// <typeparam name="TKey">Queue key type</typeparam>
	/// <typeparam name="TItem">Queue value type</typeparam>
	public interface IAddressedMultiQueueWorker<in TKey, in TItem>
	{
		/// <summary>
		/// Adds data with needed address and with needed priority in queue
		/// </summary>
		/// <param name="address">Address of element</param>
		/// <param name="item">Value of element</param>
		/// <param name="queueNumber">Number of queue (usually using as priority)</param>
		/// <returns>Unique identifier of the element in queue</returns>
		Guid AddWork(TKey address, TItem item, int queueNumber);

		/// <summary>
		/// Removes element from queue
		/// </summary>
		/// <param name="id">Unique identifier of the removing element</param>
		/// <returns>True if removed</returns>
		bool RemoveItem(Guid id);
	}
}