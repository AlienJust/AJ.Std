using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace AJ.Std.Mvvm
{
	public class RelayCommand : ICommand {
		private readonly Action _execute;

		private readonly Func<bool> _canExecute;

		public RelayCommand(Action execute) : this(execute, null) {
		}

		public RelayCommand(Action execute, Func<bool> canExecute) {
			_execute = execute ?? throw new ArgumentNullException("execute");
			_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged;

		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "This cannot be an event")]
		public void RaiseCanExecuteChanged() {
			var handler = CanExecuteChanged;
			handler?.Invoke(this, EventArgs.Empty);
		}

		public bool CanExecute(object parameter) {
			return _canExecute == null || _canExecute();
		}

		public void Execute(object parameter) {
			if (CanExecute(parameter)) {
				_execute();
			}
		}
	}
}