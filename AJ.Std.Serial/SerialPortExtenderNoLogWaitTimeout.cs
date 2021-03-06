﻿using System;
using System.Diagnostics;
using System.Threading;
using RJCP.IO.Ports;

namespace AJ.Std.Serial
{
	/// <summary>
	/// Waits for timeout even if data was already read
	/// </summary>
	public sealed class SerialPortExtenderNoLogWaitTimeout : ISerialPortExtender {
		private readonly SerialPortStream _port;
		private readonly Stopwatch _readEplasedTimer = new Stopwatch();
		private const int ReadTimeoutMs = 25;

		public SerialPortExtenderNoLogWaitTimeout(SerialPortStream port) {
			_port = port;
		}

		public void WriteBytes(byte[] bytes, int offset, int count) {
			_port.DiscardOutBuffer();
			ReadAllBytes();
			_port.Write(bytes, offset, count);
		}

		public byte[] ReadBytes(int bytesCount, TimeSpan timeout, bool discardRemainingBytesAfterSuccessRead) {
			var inBytes = new byte[bytesCount];
			int totalReadedBytesCount = 0;

			TimeSpan beetweenIterationPause = TimeSpan.FromMilliseconds(25);
			var totalIterationsCount = (int)(timeout.TotalMilliseconds / beetweenIterationPause.TotalMilliseconds);

			for (int i = 0; i < totalIterationsCount; ++i) {
				_readEplasedTimer.Restart();
				var bytesToRead = _port.BytesToRead;
				if (bytesToRead != 0) {
					var currentReadedBytesCount = _port.Read(inBytes, totalReadedBytesCount, bytesCount - totalReadedBytesCount);
					totalReadedBytesCount += currentReadedBytesCount;
					if (totalReadedBytesCount == inBytes.Length) {
						if (discardRemainingBytesAfterSuccessRead) {
							ReadAllBytes();
						}
						Thread.Sleep((totalIterationsCount - i) * ReadTimeoutMs);
						return inBytes;
					}
				}
				_readEplasedTimer.Stop();
				var sleepTime = beetweenIterationPause - _readEplasedTimer.Elapsed;
				if (sleepTime.TotalMilliseconds > 0) Thread.Sleep(sleepTime);
			}
			ReadAllBytes();
			throw new TimeoutException("ReadFromPort timeout");
		}

		public byte[] ReadAllBytes() {
			var bytesToRead = _port.BytesToRead;
			var result = new byte[bytesToRead];
			_port.Read(result, 0, bytesToRead);
			return result;
		}
	}
}