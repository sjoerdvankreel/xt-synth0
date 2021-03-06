using Microsoft.Win32;

namespace Xt.Synth0.UI
{
	public static class LoadSaveUI
	{
		const string Filter = "xs0 (*.xs0)|*.xs0";

		public static string Save()
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = Filter;
			if (dialog.ShowDialog() != true) return null;
			return dialog.FileName;
		}

		public static string Load()
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = Filter;
			if (dialog.ShowDialog() != true) return null;
			return dialog.FileName;
		}
	}
}