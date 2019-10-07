using System;

namespace AlienJust.Support.Serial
{
	public interface ISerialPortExtender {
		/// <summary>
		/// writes data bytes to port, using std write timeout
		/// </summary>
		/// <param name="bytes">bytes to write</param>
		/// <param name="offset">offset in @bytes array, from witch to start writing</param>
		/// <param name="count">count of @bytes to write</param>
		void WriteBytes(byte[] bytes, int offset, int count);
		byte[] ReadBytes(int bytesCount, TimeSpan timeout, bool discardRemainingBytesAfterSuccessRead);
		byte[] ReadAllBytes();
	}
}
