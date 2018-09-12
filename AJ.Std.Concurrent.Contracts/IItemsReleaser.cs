namespace AJ.Std.Concurrent.Contracts {
	/// <summary>
	/// Marks elements as free
	/// </summary>
	/// <typeparam name="TKey">Address type of elements</typeparam>
	public interface IItemsReleaser<in TKey>
	{

		/// <summary>
		/// Frees elements at the given address
		/// </summary>
		/// <param name="address">Address of the element that would be marked as free</param>
		void ReportSomeAddressedItemIsFree(TKey address);
	}
}