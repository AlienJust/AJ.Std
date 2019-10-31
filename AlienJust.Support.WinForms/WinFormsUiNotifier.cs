using System;
using System.Windows.Forms;
using AlienJust.Support.Concurrent.Contracts;

namespace AlienJust.Support.WinForms {
	public sealed class WinFormsUiNotifier : IThreadNotifier {
		private readonly Control _control;

		public WinFormsUiNotifier(Control control) {
			_control = control;
		}

		public void Notify(Action notifyAction) {
			if (_control.InvokeRequired) {
				_control.Invoke(notifyAction);
			}
			else notifyAction();
		}
	}
}