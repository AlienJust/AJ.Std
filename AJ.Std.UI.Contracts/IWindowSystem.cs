namespace AJ.Std.UI.Contracts
{
	public interface IWindowSystem {
		string ShowOpenFileDialog(string dialogTitle, string filter);
		string ShowSaveFileDialog(string dialogTitle, string filter);
		void ShowMessageBox(string message, string caption);
		BinaryChoise ShowYesNoDialog(string message, string caption);
	}
}