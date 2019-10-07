using System;
using System.IO;
using AlienJust.Support.Loggers.Contracts;

namespace AlienJust.Support.Loggers {
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
