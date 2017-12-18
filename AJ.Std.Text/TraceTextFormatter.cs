using System;
using System.Diagnostics;
using System.Threading;
using AJ.Std.Text.Contracts;

namespace AJ.Std.Text
{
	public class TraceTextFormatter : ITextFormatter {
		private readonly string _seporator;
		private readonly int _frameIndex;
		private readonly string _fileNameLimiter;
		private readonly int _framesCountMax;

		public TraceTextFormatter(string seporator, int stackFrameOffset, string fileNameLimiter) {
			_seporator = seporator;
			_frameIndex = stackFrameOffset;
			_framesCountMax = int.MaxValue;
			_fileNameLimiter = fileNameLimiter;
		}

		public TraceTextFormatter(string seporator, int stackFrameOffset, int framesCountMax, string fileNameLimiter) {
			_seporator = seporator;
			_frameIndex = stackFrameOffset;
			_framesCountMax = framesCountMax;
			_fileNameLimiter = fileNameLimiter;
		}


		public string Format(string text) {
			if (text == string.Empty) return text;

			var threadStr = Thread.CurrentThread.ManagedThreadId;
			var timeStr = DateTime.Now.ToString("HH:mm:ss.fff");


			var stackTrace = new StackTrace(true);
			var stackDeep = stackTrace.FrameCount < _frameIndex + _framesCountMax ? stackTrace.FrameCount : _frameIndex + _framesCountMax;

			var stackStr = string.Empty;
			for (int i = stackDeep - 1; i >= _frameIndex; --i) {
				stackStr += FrameToString(stackTrace.GetFrame(i), _seporator);
			}

			var lastFrame = stackTrace.GetFrame(_frameIndex);

			var fileStr = GetShortenFileName(lastFrame.GetFileName(), _fileNameLimiter) + ":" + lastFrame.GetFileLineNumber();
			string outStr = timeStr + _seporator +
											fileStr + _seporator +
											stackStr + // _seporator + // already ended with _seporator
											threadStr + _seporator +
											text;


			return outStr;
		}


		private static string FrameToString(StackFrame frame, string seporator) {
			var result = string.Empty;
			var m = frame.GetMethod();
			var t = m.DeclaringType;
			if (t != null)
				result += t.Name + ".";
			result += m.Name + seporator;
			return result;
		}


		private static string GetShortenFileName(string fileName, string limiter) {
			try {
				var lastFoundLimiterPos = fileName.LastIndexOf(limiter, StringComparison.Ordinal);
				if (lastFoundLimiterPos < 0) return fileName;

				return fileName.Substring(lastFoundLimiterPos + limiter.Length);
			}
			catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}
			return fileName;
		}
	}
}