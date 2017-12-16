using System.Collections.Generic;

namespace AJ.Std.Collections.Contracts {
	public interface ISequencePartitioner<out T> {
		IEnumerable<T> GetNextPart();
	}
}
