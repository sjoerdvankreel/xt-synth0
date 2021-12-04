using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Xt.Synth0.UI
{
	public static class MenuUI
	{
		public static event EventHandler New;
		public static event EventHandler Open;
		public static event EventHandler Save;
		public static event EventHandler SaveAs;
		public static event EventHandler Settings;

		public static UIElement Make()
		{
			var result = new Menu();
			result.Items.Add(MakeFile());
			return result;
		}

		static MenuItem MakeItem(string header)
		{
			var result = new MenuItem();
			result.Header = header;
			return result;
		}

		static MenuItem MakeItem(
			ICommand command, string header, Action execute)
		{
			var result = MakeItem(header);
			result.Command = command;
			var binding = new CommandBinding();
			binding.Command = command;
			binding.Executed += (s, e) => execute();
			result.CommandBindings.Add(binding);
			return result;
		}

		static UIElement MakeFile()
		{
			var result = MakeItem("_File");
			var doNew = () => New(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.New, "_New", doNew));
			var doOpen = () => Open(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.Open, "_Open", doOpen));
			result.Items.Add(new Separator());
			var doSave = () => Save(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.Save, "_Save", doSave));
			var doSaveAs = () => SaveAs(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.SaveAs, "Save _As", doSaveAs));
			result.Items.Add(new Separator());
			var doSettings = () => Settings(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.SaveAs, "Settings", doSettings));
			return result;
		}
	}
}