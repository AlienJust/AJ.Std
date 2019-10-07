using System.Collections.Generic;

namespace AlienJust.Support.Collections.Contracts {
	public interface ISequencePartitioner<out T> {
		IEnumerable<T> GetNextPart();
	}
}
