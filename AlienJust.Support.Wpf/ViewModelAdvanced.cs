using System.Windows.Input;
using AlienJust.Support.Mvvm;

namespace AlienJust.Support.Wpf
{
	public class ViewModelAdvanced : ViewModelBase
	{
		public CommandBindingCollection CommandBindings { get; } = new CommandBindingCollection();
	}
}
