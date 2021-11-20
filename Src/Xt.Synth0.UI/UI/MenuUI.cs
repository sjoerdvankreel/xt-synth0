using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Xt.Synth0.UI
{
	public static class MenuUI
	{
		public static event EventHandler Load;
		public static event EventHandler Save;
		public static event EventHandler SaveAs;

		public static UIElement Make()
		{
			var result = new Menu();
			result.Items.Add(MakeFile());
			return result;
		}

		static UIElement MakeFile()
		{
			var result = UI.MakeItem("_File");
			var doOpen = () => Load(null, EventArgs.Empty);
			result.Items.Add(UI.MakeItem(ApplicationCommands.Open, "_Open", doOpen));
			var doSave = () => Save(null, EventArgs.Empty);
			result.Items.Add(UI.MakeItem(ApplicationCommands.Save, "_Save", doSave));
			var doSaveAs = () => SaveAs(null, EventArgs.Empty);
			result.Items.Add(UI.MakeItem(ApplicationCommands.SaveAs, "Save _As", doSaveAs));
			return result;
		}
	}
}