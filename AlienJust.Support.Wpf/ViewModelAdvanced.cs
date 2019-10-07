using System.Windows.Input;
using AlienJust.Support.Mvvm;

namespace AlienJust.Adaptation.WindowsPresentation
{
    public class ViewModelAdvanced : ViewModelBase
    {
        public CommandBindingCollection CommandBindings { get; } = new CommandBindingCollection();
    }
}
