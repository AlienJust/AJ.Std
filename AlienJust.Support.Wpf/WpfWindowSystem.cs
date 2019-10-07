using System;
using System.Windows;
using AlienJust.Support.UI.Contracts;
using Microsoft.Win32;

namespace AlienJust.Adaptation.WindowsPresentation
{
    public sealed class WpfWindowSystem : IWindowSystem
    {
        public string ShowOpenFileDialog(string dialogTitle, string filter)
        {
            var dialog = new OpenFileDialog { Title = dialogTitle, Filter = filter };
            var result = dialog.ShowDialog();

            if (result != null && result.Value)
            {
                return dialog.FileName;
            }
            return null;
        }

        public string ShowSaveFileDialog(string dialogTitle, string filter)
        {
            var dialog = new SaveFileDialog { Title = dialogTitle, Filter = filter };
            var result = dialog.ShowDialog();

            if (result != null && result.Value)
            {
                return dialog.FileName;
            }
            return null;
        }

        public void ShowMessageBox(string message, string caption)
        {
            MessageBox.Show(message, caption);
        }

        public BinaryChoise ShowYesNoDialog(string message, string caption)
        {
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    return BinaryChoise.Yes;
                case MessageBoxResult.No:
                    return BinaryChoise.No;
                default:
                    throw new Exception("Unexpected dialog result");
            }
        }
    }
}
