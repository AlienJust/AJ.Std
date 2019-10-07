using System;
using System.Runtime.InteropServices;

namespace AlienJust.Adaptation.Console {
	public sealed class ConsoleControl
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool FreeConsole();

		[DllImport("kernel32", SetLastError = true)]
		static extern bool AttachConsole(int dwProcessId);

		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

		public static void InvokeAllocConsole()
		{
			AllocConsole();
		}

		public static void InvokeFreeConsole()
		{
			FreeConsole();
		}
	}
}
