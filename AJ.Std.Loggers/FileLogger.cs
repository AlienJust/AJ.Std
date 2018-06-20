using System;
using System.IO;
using AJ.Std.Loggers.Contracts;

namespace AJ.Std.Loggers {
	public class FileLogger : ILogger {
		private readonly object _sync = new object();
		private readonly string _filename;

		public FileLogger(string filename) {
			_filename = filename;
		}

		public void Log(string text) {
			lock (_sync) {
				try {
					File.AppendAllText(_filename, text + Environment.NewLine);
				}
				catch {
				}
			}
		}

		public void Log(object obj) {
			Log(obj.ToString());
		}
	}
}
