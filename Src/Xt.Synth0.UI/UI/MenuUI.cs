using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Xt.Synth0.UI
{
	public static class MenuUI
	{
		public static Action New;
		public static Action Load;
		public static Action Save;
		public static Action SaveAs;

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

		static MenuItem MakeItem(ICommand command, string header, Func<Action> execute)
		{
			var result = MakeItem(header);
			result.Command = command;
			var binding = new CommandBinding();
			binding.Command = command;
			binding.Executed += (s, e) => execute()();
			result.CommandBindings.Add(binding);
			return result;
		}

		static UIElement MakeFile()
		{
			var result = MakeItem("_File");
			result.Items.Add(MakeItem(ApplicationCommands.New, "_New", () => New));
			result.Items.Add(MakeItem(ApplicationCommands.Open, "_Open", () => Load));
			result.Items.Add(new Separator());
			result.Items.Add(MakeItem(ApplicationCommands.Save, "_Save", () => Save));
			result.Items.Add(MakeItem(ApplicationCommands.SaveAs, "Save _As", () => SaveAs));
			return result;
		}
	}
}